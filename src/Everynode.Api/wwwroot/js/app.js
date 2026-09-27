async function startSession() {
    const sessionEndpoint = "/session";
    const response = await fetch(sessionEndpoint, {method: "POST"});

    if (!response.ok) {
        throw new Error(`Response status: ${response.status}`);
    }

    return await response.json();
}

async function getField(){
    const fieldEndpoint = "/field";

    const response = await fetch(fieldEndpoint);

    if (!response.ok) {
        throw new Error(`Response status: ${response.status}`);
    }

    return await response.json()
}

async function sendNode(x, y){
    const sendNodeEndpoint = "/addNode";
    
    const response = await fetch(sendNodeEndpoint, 
        {method: "POST",
            headers:{
            "Content-Type": "application/json"
            },
            body: JSON.stringify({ x, y }),
        });
    
    if (!response.ok) {
        const message = await response.text();
        throw new Error(message || `Response status: ${response.status}`);
    }
}

async function handleCanvasClick(event) {
    const svg = event.currentTarget;

    // Пока реагируем только на пустое место, не на вершину или ребро.
    if (event.target !== svg) return;

    const matrix = svg.getScreenCTM();
    if (matrix === null) return;

    const point = new DOMPoint(event.clientX, event.clientY)
        .matrixTransform(matrix.inverse());

    const viewBox = svg.viewBox.baseVal;
    const insideField =
        point.x >= viewBox.x &&
        point.x <= viewBox.x + viewBox.width &&
        point.y >= viewBox.y &&
        point.y <= viewBox.y + viewBox.height;

    if (!insideField) return;
    
    try {
        await sendNode(point.x, point.y);
        
        await refreshView(svg)
    } catch (e) {
        console.error(e.message);
    }
    
}

function renderEdges(edges, nodes, svg){
    edges.forEach((edge) => {
        let x1;
        let y1;

        let x2;
        let y2;

        nodes.forEach(node => {
            if (edge.startId === node.id) {x1 = node.x; y1 = node.y;}
            if (edge.endId === node.id) {x2 = node.x; y2 = node.y;}
        })

        const line = document.createElementNS(
            "http://www.w3.org/2000/svg",
            "line"
        );

        line.setAttribute("x1", x1);
        line.setAttribute("y1", y1);
        line.setAttribute("x2", x2);
        line.setAttribute("y2", y2);
        line.setAttribute("stroke", "#64748b");
        line.setAttribute("stroke-width", "2");

        svg.appendChild(line);
    })
}

function renderNodes(nodes, svg){
    nodes.forEach((element) => {
        const circle = document.createElementNS(
            "http://www.w3.org/2000/svg",
            "circle"
        );

        circle.setAttribute("cx", `${element.x}`);
        circle.setAttribute("cy", `${element.y}`);
        circle.setAttribute("r", "8");
        circle.setAttribute("fill", "#2563eb");

        svg.appendChild(circle);
    })
}

function renderField(edges, nodes, svg) {
    renderEdges(edges, nodes, svg);

    renderNodes(nodes, svg);
}

async function refreshView(svg){
    const loadingHeaderField = document.getElementById("LoadingHeader");

    const idField = document.getElementById("UserID");
    const nodesBudgetField = document.getElementById("UserNodesBudget");
    const edgesBudgetField = document.getElementById("UserEdgesBudget");

    const nodesCountField = document.getElementById("NodesCount");
    const edgesCountField = document.getElementById("EdgesCount");
    
    try{
        const sessionData = await startSession()
        const fieldData = await getField()

        idField.textContent = sessionData.id;
        nodesBudgetField.textContent = sessionData.nodesBudget;
        edgesBudgetField.textContent = sessionData.edgesBudget;

        nodesCountField.textContent = fieldData.nodes.length;
        edgesCountField.textContent = fieldData.edges.length;

        loadingHeaderField.textContent = "Successfully Loaded"
        
        svg.replaceChildren()
        
        renderField(fieldData.edges, fieldData.nodes, svg);


    } catch (e){
        loadingHeaderField.textContent = "Error Loading"

        console.error(e.message)
    }
}

async function startApp(){
    const svg = document.getElementById("GraphCanvas");
    
    svg.addEventListener("click", handleCanvasClick);
    
    await refreshView(svg)
}

startApp();
