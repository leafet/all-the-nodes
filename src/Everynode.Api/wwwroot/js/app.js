async function startSession() {
    const sessionEndpoint = "/session";
    try{
        const response = await fetch(sessionEndpoint, {method: "POST"});

        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const responseJSON = await response.json();
        console.log(responseJSON.id + " " + responseJSON.nodesBudget + " " + responseJSON.edgesBudget);
    } catch (error) {
        console.error(error.message);
    }
}

async function getField(){
    const fieldEndpoint = "/field";

    try{
        const response = await fetch(fieldEndpoint);

        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        return await response.json()
    } catch (error) {
        console.error(error.message);
    }
}

async function startApp(){
    await startSession()
    await getField()
}

startApp();
