using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98.Components
{
    internal class SupabaseManager
    {
        private const string APIKey = "sb_publishable_mE21VqrdKRtmL1RKLGMnaw_aY-nAr9C";
        private const string baseURL = "https://swnnzdwzutxscvqbbsmz.supabase.co";

        /// <summary>
        /// Load leaderboard thông qua supabase
        /// </summary>
        public static async Task LoadLeaderboard(Control board)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Add("apikey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {APIKey}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                // Lấy top 10 người chơi cao điểm nhất
                string url = $"rest/v1/PLAYER?select=*&order=highscore.desc&limit=10";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JArray data = JArray.Parse(json);

                    // Bỏ danh sách hiện tại
                    if (board.InvokeRequired)
                    {
                        board.Invoke(new Action(() => board.Controls.Clear()));
                    }
                    else
                    {
                        board.Controls.Clear();
                    }

                    // Add players to leaderboard
                    for (int i = 0; i < data.Count; i++)
                    {
                        JObject user = (JObject)data[i];
                        string UserName = (string)user["name"];
                        int UserTimePlayed = (int)user["time_played"];
                        int UserScore = (int)user["highscore"];

                        Player player = new Player(UserName, UserScore, UserTimePlayed);
                        PlayerInfo playerinfo = new PlayerInfo(player, i + 1);

                        if (board.InvokeRequired)
                        {
                            board.Invoke(new Action(() => board.Controls.Add(playerinfo)));
                        }
                        else
                        {
                            board.Controls.Add(playerinfo);
                        }
                    }
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Lấy thông tin thất bại! Error: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm người chơi hiện tại vào database
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>Nếu người chơi có tên trong database thì chỉ cập nhật lại thời gian chơi và điểm số(Nếu điểm hiện tạo cao hơn điểm cũ)</description></item>
        /// <item><description>Không có tên trong database thì thêm mới</description></item>
        /// </list>
        /// </remarks>
        public static async Task SaveOrUpdatePlayer(Player player)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Add("apikey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {APIKey}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                // Kiểm tra xem có tồn tại người chơi không
                string checkUrl = $"rest/v1/PLAYER?name=eq.{Uri.EscapeDataString(player.Name)}&select=id,highscore,time_played";
                HttpResponseMessage checkResponse = await client.GetAsync(checkUrl);

                if (checkResponse.IsSuccessStatusCode)
                {
                    string checkJson = await checkResponse.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(checkJson) && checkJson != "[]")
                    {
                        JArray existingPlayers = JArray.Parse(checkJson);

                        if (existingPlayers.Count > 0)
                        {
                            // Có người chơi thì cập nhật lại highscore và thời gian chơi
                            JObject existingPlayer = (JObject)existingPlayers[0];
                            int existingHighscore = (int)existingPlayer["highscore"];
                            int existingTimePlayed = (int)existingPlayer["time_played"];

                            // Chỉ cập nhật khi điểm hiện tại cao hơn điểm cũ
                            if (player.HighestScore > existingHighscore)
                            {
                                // Cập nhật thành điểm cao mới
                                string updateUrl = $"rest/v1/PLAYER?name=eq.{Uri.EscapeDataString(player.Name)}";

                                var updateData = new
                                {
                                    highscore = player.HighestScore,
                                    time_played = existingTimePlayed + player.TimePlayed
                                };

                                string jsonData = JsonConvert.SerializeObject(updateData);
                                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                                var request = new HttpRequestMessage(new HttpMethod("PATCH"), updateUrl)
                                {
                                    Content = content
                                };

                                HttpResponseMessage updateResponse = await client.SendAsync(request);

                                if (!updateResponse.IsSuccessStatusCode)
                                {
                                    string error = await updateResponse.Content.ReadAsStringAsync();
                                    MessageBox.Show($"Update failed: {error}");
                                }
                            }
                            else
                            {
                                // Điểm không cao hơn thì chỉ cập nhật thời gian chơi
                                string updateUrl = $"rest/v1/PLAYER?name=eq.{Uri.EscapeDataString(player.Name)}";

                                var updateData = new
                                {
                                    time_played = existingTimePlayed + player.TimePlayed
                                };

                                string jsonData = JsonConvert.SerializeObject(updateData);
                                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                                var request = new HttpRequestMessage(new HttpMethod("PATCH"), updateUrl)
                                {
                                    Content = content
                                };

                                await client.SendAsync(request);
                            }
                        }
                        else
                        {
                            // Không tồn tại người chơi thì thêm mới người chơi
                            await InsertNewPlayer(client, player);
                        }
                    }
                    else
                    {
                        // Không tìm thấy người chơi thì thêm mới người chơi
                        await InsertNewPlayer(client, player);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save/Update error: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm mới người chơi vào database
        /// </summary>
        private static async Task InsertNewPlayer(HttpClient client, Player player)
        {
            string insertUrl = "rest/v1/PLAYER";

            var insertData = new
            {
                name = player.Name,
                highscore = player.HighestScore,
                time_played = player.TimePlayed
            };

            string jsonData = JsonConvert.SerializeObject(insertData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage insertResponse = await client.PostAsync(insertUrl, content);

            if (!insertResponse.IsSuccessStatusCode)
            {
                string error = await insertResponse.Content.ReadAsStringAsync();
                MessageBox.Show($"Insert failed: {error}");
            }
        }
    }
}
