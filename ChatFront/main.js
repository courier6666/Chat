var socket = new WebSocket("ws://127.0.0.1:7171");
socket.addEventListener("open", function () {
    console.log("Connected");

});

let lastMessageTime = new Date();

let messageListenerFunction = function (event) {
    let message = JSON.parse(event.data);
    console.log(message);
    if(message.messageType == 'identification')
    {
        if(!sessionStorage.getItem('id'))
            sessionStorage.setItem('id', message.providedId);

        fetch(`http://localhost:7071/messages?timeBefore=${lastMessageTime.toISOString()}`)
            .then(response => response.json())
            .then(data => console.log(data)); 

        messageListenerFunction = function (event) {
            let message = JSON.parse(event.data);
            console.log(message);
            let output = document.getElementById('output');
            output.appendChild(createParagraph(message.data));
        };
    }
};

socket.addEventListener("message", function(event){
    messageListenerFunction(event);
});



function createParagraph(content) {
    var p = document.createElement('p');
    p.innerText = content;
    return p;
}

function createMessage() {
    let input = document.getElementById('messageInput').value;

    let message = {
        data: input,
        messageType: "basic",
        authorId: sessionStorage.getItem('id')
    };

    return message;
}

function onInputChange(){
    let input = document.getElementById('messageInput');
    let button = document.getElementById('sendButton');
    if(input.value == '')
    {
        button.disabled = true;
    }
    else {
        button.disabled = false;
    }
}

function buttonClick() {
    let jsonString = JSON.stringify(createMessage());

    socket.send(jsonString);
}