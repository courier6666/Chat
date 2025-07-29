using System.Net;
using System.Text.Json;
using Chat.DatabaseAccess;

namespace Chat.ChatServer;

public delegate T EndpointDelegate<out T>(HttpListenerContext context);

public class ChatHttpServer : IDisposable
{
    private HttpListener listener;
    private IMessageService messageService;
    private Dictionary<string, EndpointDelegate<object>> endpoints = [];
    private JsonSerializerOptions options;
    
    public ChatHttpServer(string prefix, IMessageService  messageService)
    {
        this.listener = new HttpListener();
        this.listener.Prefixes.Add(prefix);
        
        this.messageService = messageService;
    }

    public void AddEndpoint<TReturnType>(string route, EndpointDelegate<TReturnType> endpoint)
    {
        endpoints[route] = (context) => (object)endpoint(context)!;
    }

    
    public void Stop() => this.listener.Stop();
    
    public async Task StartAsync()
    {
        this.listener.Start();
        while (true)
        {
            var context = await this.listener.GetContextAsync();
            Task.Run(() => ProcessRequestAsync(context));
            await Task.Delay(5);
        }
    }
    
    public async Task ProcessRequestAsync(HttpListenerContext context)
    {
        //TODO http request implementation
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        listener.Close();
    }
}