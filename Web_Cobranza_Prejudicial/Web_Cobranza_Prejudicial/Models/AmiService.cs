using System.Net.Sockets;
using System.Text;

public class AmiService
{
    private TcpClient _client;
    private NetworkStream _stream;

    public async Task ConnectAsync()
    {
        _client = new TcpClient();
        await _client.ConnectAsync("192.168.0.61", 5038);

        _stream = _client.GetStream();

        string login =
            "Action: Login\r\n" +
            "Username: admin\r\n" +
            "Secret: Padre123\r\n\r\n";

        await SendAsync(login);
    }

    public async Task<string> ClickToCall(string anexo, string numero)
    {
        string action =
                "Action: Originate\r\n" +
                $"Channel: SIP/{anexo}\r\n" +
                "Context: from-internal\r\n" +
                $"Exten: {numero}\r\n" +
                "Priority: 1\r\n" +
                $"CallerID: {anexo}\r\n" +
                "Async: true\r\n\r\n";

        return await SendAsync(action);
    }

    private async Task<string> SendAsync(string data)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        await _stream.WriteAsync(buffer);

        byte[] responseBuffer = new byte[4096];
        int bytes = await _stream.ReadAsync(responseBuffer);

        return Encoding.ASCII.GetString(responseBuffer, 0, bytes);

    }
}
