let pointerStartedOnNode = false;

const loadingHeaderField = document.getElementById("LoadingHeader");

const idField = document.getElementById("UserID");
const nodesBudgetField = document.getElementById("UserNodesBudget");
const edgesBudgetField = document.getElementById("UserEdgesBudget");

const nodesCountField = document.getElementById("NodesCount");
const edgesCountField = document.getElementById("EdgesCount");

const errorField = document.getElementById("ErrorHolder");

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

async function sendEdge(StartId, EndId){
    const sendEdgeEndpoint = "/addEdge";

    const response = await fetch(sendEdgeEndpoint,
        {method: "POST",
            headers:{
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ StartId, EndId }),
        });

    if (!response.ok) {
        const message = await response.text();
        throw new Error(message || `Response status: ${response.status}`);
    }
}

async function handleCanvasClick(event) {
    if (pointerStartedOnNode) return;
    
    const svg = event.currentTarget;

    if (event.target instanceof SVGCircleElement) {
        const nodeId = Number(event.target.dataset.nodeId);
        console.log("Clicked on node:", nodeId);
        return;
    }
    
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

        errorField.textContent = e.message;
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
        circle.setAttribute("data-node-id", element.id);

        svg.appendChild(circle);
    })
}

function renderField(edges, nodes, svg) {
    renderEdges(edges, nodes, svg);

    renderNodes(nodes, svg);
}

async function refreshView(svg){
    
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
        
        errorField.textContent = e.message;
    }
}

function setupEdgeGesture(svg){
    
    let startNodeId = null;
    
    svg.addEventListener("pointerdown", event => {
        pointerStartedOnNode = event.target instanceof SVGCircleElement;
        
        if (!pointerStartedOnNode) return;
        
        startNodeId = Number(event.target.dataset.nodeId);
        console.log("Start:", startNodeId);
    })
    
    window.addEventListener("pointerup", async event => {
        if (startNodeId === null) return;

        const target = document.elementFromPoint(
            event.clientX,
            event.clientY
        )

        if (target instanceof SVGCircleElement) {
            const endNodeId = Number(target.dataset.nodeId);

            if (endNodeId !== startNodeId) {
                await sendEdge(startNodeId, endNodeId);
                
                await refreshView(svg);
            }
        }

        startNodeId = null

    })

    window.addEventListener("pointercancel", () => {
        startNodeId = null;
    })

    window.addEventListener("click", () => {
        pointerStartedOnNode = false;
    });
}

async function startApp(){
    const svg = document.getElementById("GraphCanvas");
    
    svg.addEventListener("click", handleCanvasClick);
    
    setupEdgeGesture(svg);
    
    await refreshView(svg)
}

startApp();
