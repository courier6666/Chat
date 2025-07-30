using System.Net;
using System.Text.Json;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chat.ChatServer;

public delegate Task<T> EndpointDelegate<T>(HttpListenerContext context);

/// <summary>
/// Http server that sends json-only files.
/// </summary>
public class ChatHttpServer : IDisposable
{
    private HttpListener listener;
    private Dictionary<string, EndpointDelegate<object>> endpoints = [];
    private JsonSerializerOptions options;
    
    public ChatHttpServer(string prefix, JsonSerializerOptions? options = null)
    {
        this.listener = new HttpListener();
        this.listener.Prefixes.Add(prefix);
        this.options = options ?? new JsonSerializerOptions();
    }

    public void AddEndpoint<TReturnType>(string route, EndpointDelegate<TReturnType> endpoint)
    {
        endpoints[route] = async (context) => (object)(await endpoint(context))!;
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
            var endpoint = 
                (this.endpoints.FirstOrDefault(i => context.Request.RawUrl.StartsWith(i.Key))).
                    Value;

            if (endpoint == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
                return;
            }
            
            var res = await endpoint(context);
            var json = JsonSerializer.Serialize(res, this.options);
            var encoded = Encoding.UTF8.GetBytes(json);
            context.Response.ContentLength64 = encoded.Length;
            
            using var stream = context.Response.OutputStream;
            context.Response.OutputStream.Write(encoded, 0, encoded.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";
            
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