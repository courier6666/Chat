var socket = new WebSocket("ws://127.0.0.1:7171");
socket.addEventListener("open", function () {
    console.log("Connected");

});

socket.addEventListener("message", function (event) {
    let message = JSON.parse(event.data);
    console.log(message);
    if(message.messageType == 'identification')
    {
        if(!sessionStorage.getItem('id'))
            sessionStorage.setItem('id', message.data);
    }
    else {
        let output = document.getElementById('output');
        output.appendChild(createParagraph(message.data));
    }
    
});


function createParagraph(content) {
    var p = document.createElement('p');
    console.log(content);
    p.innerText = content;
    return p;
}

function createMessage() {
    let message = {
        data: "Hello",
        messageType: "basic",
        authorId: sessionStorage.getItem('id')
    };

    return message;
}

function buttonClick() {
    let jsonString = JSON.stringify(createMessage());
    socket.send(jsonString);
}
