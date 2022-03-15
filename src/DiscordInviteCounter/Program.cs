using System.Runtime.InteropServices;
using Tommy;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordInviteCounter
{
    class Program
    {
        public ActivityType ac;

        private Config conf = new Config();
        
        private readonly DiscordSocketClient _client;

        public static void Main()
        {
                        
            

            
            new Program().MainAsync().GetAwaiter().GetResult();
        }
    
        
        public Program()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.UserJoined += OnPlayerJoinAsync;
        }

        public async Task MainAsync()
        {
            using (StreamReader reader = File.OpenText("config.toml"))
            {
                TomlTable table = TOML.Parse(reader);
                Console.WriteLine("Reading configuration succesfull");

                //connection
                conf.Token = table["Connection"]["token"];
                Console.WriteLine(conf.Token);
                //users
                //conf.Admin = table["Users"]["admins"];
                conf.Status = table["Other"]["status"];
                conf.StatusSuffix = table["Other"]["statussuffix"];
                foreach (TomlString t in table["Ranks"])
                {
                    Console.WriteLine(t.ToString());
                }

            }
            
            await _client.LoginAsync(TokenType.Bot, conf.Token);
            await _client.StartAsync();
        
            // Block the program until it is closed.
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            switch (conf.StatusSuffix)
            {
                case 0:
                    ac = ActivityType.Playing;
                    break;
                case 1:
                    ac = ActivityType.Listening;
                    break;
                case 2:
                    ac = ActivityType.Competing;
                    break;
                case 3:
                    ac = ActivityType.Streaming;
                    break;
                case  4:
                    ac = ActivityType.Watching;
                    break;
            }
            
           _client.SetActivityAsync(new Game( conf.Status, ac));

            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "test zaproszenia",
                Description = "description jsadabsdj"
            };
            embed.WithAuthor(message.Author);
            embed.WithColor(Color.Teal);
            
            if (message.Content.StartsWith(".top"))
            {
                var chnl = message.Channel as SocketGuildChannel;
                var array = await chnl.Guild.GetInvitesAsync();
                string[] args = message.Content.Split(" ");
                
                
                /*
                await message.Channel.SendMessageAsync(args[1]);
                int argint = Int32.Parse(args[1]);
                for (int b = 0; b > argint; b++)
                {
                    
                }
*/
                //_client.GetInviteAsync()
                ///
                RestInviteMetadata a;
                foreach (RestInviteMetadata i in array)
                {
                    embed.AddField(i.Inviter.Username + "#" + i.Inviter.Discriminator , i.Uses + " " + i.Id);
                    Console.WriteLine(i.Uses);
                    Console.WriteLine(i.Inviter);
                }

                await message.Channel.SendMessageAsync(embed: embed.Build());

            }
        }

        private async Task OnPlayerJoinAsync(SocketGuildUser user)
        {
        }
    }
}