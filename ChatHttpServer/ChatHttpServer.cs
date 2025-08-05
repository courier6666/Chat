using System.Net;
using System.Text.Json;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Chat.ChatServer.Endpoint;
using Chat.ChatServer.Router;
using Chat.ChatServer.Endpoint.Attributes;
using Chat.ChatServer.Extensions;

namespace Chat.ChatServer;

/// <summary>
/// Http server that sends json-only files.
/// </summary>
public class ChatHttpServer : IDisposable
{
    private HttpListener listener;
    private Router.Router router;
    private JsonSerializerOptions options;
    
    internal JsonSerializerOptions JsonSerializerOptions => options;
    
    public ChatHttpServer(string prefix, JsonSerializerOptions? options = null)
    {
        this.router = new Router.Router();
        this.listener = new HttpListener();
        this.listener.Prefixes.Add(prefix);
        this.options = options ?? new JsonSerializerOptions();
    }

    public void AddEndpoint(HttpServerEndpoint endpoint)
    {
        endpoint.ChatHttpServer = this;
        this.router.AddRoute(endpoint);
    }
    
    public void Stop() => this.listener.Stop();
    
    public async Task StartAsync()
    {
        this.listener.Start();
        while (true)
        {
            try
            {
                var context = await this.listener.GetContextAsync();
                Task.Run(() => ProcessRequestAsync(context));
                await Task.Delay(5);
            }
            catch (HttpListenerException) { break; }
            catch (InvalidOperationException) { break; }
        }
    }
    
    public async Task ProcessRequestAsync(HttpListenerContext context)
    {
        try
        {
            var endpoint = this.router.GetRoute(context.GetRouteParamsFromHttpContext());

            if (endpoint == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
                return;
            }
            
            var res = await endpoint.Execute(context);
            
            context.Response.ContentLength64 = res.Data.Length;
            
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            
            using var stream = context.Response.OutputStream;
            await context.Response.OutputStream.WriteAsync(res.Data, 0, res.Data.Length);
            context.Response.StatusCode = (int)res.StatusCode;
            context.Response.ContentType = res.ContentType;
            
            context.Response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error has occured during request processing: " + ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Close();
        }
    }
    
    public void Dispose()
    {
        listener.Close();
    }
}