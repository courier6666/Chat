var socket = new WebSocket("ws://127.0.0.1:7171");
socket.addEventListener("open", function () {
    console.log("Connected");

});

async function loadMessagesBeforeDateTime(count)
{
    let res = await fetch(`http://localhost:7071/messages?timeBefore=${lastMessageTime.toISOString()}&count=${count}`)
        .then(response => response.json());
    
    if(res.length > 0)
        lastMessageTime = new Date(res[0].timeUtc);

    return res;
}

function displayMessagesToOutput(messages)
{
    let output = document.getElementById('output');

    for (let i = messages.length - 1; i >= 0; --i)
    {
        output.prepend(createMessageHtml(messages[i]));
    }
}


let lastMessageTime = new Date();

let messageListenerFunction = function (event) {
    let message = JSON.parse(event.data);
    console.log(message);
    if(message.messageType == 'identification')
    {
        if(!sessionStorage.getItem('id'))
            sessionStorage.setItem('id', message.providedId);

        loadMessagesBeforeDateTime(20).then(val => displayMessagesToOutput(val));

        messageListenerFunction = function (event) {
            let message = JSON.parse(event.data);
            let output = document.getElementById('output');

            let isScrolledToBotttom = output.scrollTop == output.scrollHeight;

            let mes = createMessageHtml(message);
            output.appendChild(mes);

            if(message.authorId == sessionStorage.getItem('id') || isScrolledToBotttom)
            {
                output.scrollTo({
                    top: output.scrollHeight,
                    behavior: 'instant'
                });
            }

        };
    }
};

socket.addEventListener("message", function(event){
    messageListenerFunction(event);
});

function createMessageHtml(message) {
    console.log(message);
    let messageWrapperDiv = document.createElement('div');
    let messageDiv = document.createElement('div');

    let username = document.createElement('p');
    username.className = 'username';
    username.innerText = message.authorId;

    let data = document.createElement('p');
    data.className = 'data';
    data.innerText = message.data;

    let time = document.createElement('p');
    time.innerText = new Date(message.timeUtc).toLocaleString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });

    messageDiv.appendChild(username);
    messageDiv.appendChild(data);
    messageDiv.appendChild(time);

    messageDiv.classList.add('message');
    //checking if message is user's message
    console.log(sessionStorage.getItem('id') == message.authorId);
    sessionStorage.getItem('id') == message.authorId ? messageDiv.classList.add('my-message') : messageDiv.classList.add('other-message');
    
    messageWrapperDiv.appendChild(messageDiv);

    return messageWrapperDiv;
}


function createMessage(content) {
    let value = content.trim();

    if(value == '')
    {
        throw new Error("Message cannot be empty!");
    }

    let message = {
        data: value,
        messageType: "basic",
        authorId: sessionStorage.getItem('id')
    };

    return message;
}

function onInputChange(){
    let input = document.getElementById('messageInput');
    let button = document.getElementById('sendButton');
    if(input.value.trim() == '')
    {
        button.disabled = true;
    }
    else {
        button.disabled = false;
    }
}

function sendMessage(content)
{
    let jsonString = JSON.stringify(createMessage(content));
    socket.send(jsonString);
}

function onInputMessageSend(){
    let input = document.getElementById("messageInput");
    sendMessage(input.value);
    input.value = '';
}

function buttonClick() {
    onInputMessageSend()
}

function buttonEnterPressedUp(event)
{
    console.log(event);
    if (event.key !== "Enter")
    {
        return;
    }

    let input = document.getElementById("messageInput");
    if (input.value.trim() == '')
    {
        return;
    }

    onInputMessageSend()
}

function createLoadingGifElement(){
    let wrapperDiv = document.createElement('div');
    wrapperDiv.id = 'loading-gif';
    wrapperDiv.style.textAlign = 'center';

    let loadingGif = document.createElement('img');
    loadingGif.src = 'assets/loading-gif.svg';
    loadingGif.className = 'loading-gif';

    wrapperDiv.append(loadingGif);
    return loadingGif;
}

function onChatScroll(event){
    let output = document.getElementById('output');

    if(!output)
    {
        throw new Error("Element with id #output not found!");
    }

    let child = output.childNodes[0];
    if(output.scrollTop == 0)
    {
        console.log(output);
        let loadingGif = createLoadingGifElement();
        output.prepend(loadingGif);

        loadMessagesBeforeDateTime(20).then(val =>
        {
            loadingGif.remove();
            displayMessagesToOutput(val);
            if(child) child.scrollIntoView();
        });
    }
}