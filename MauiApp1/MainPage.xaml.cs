using System.Text.Json;

namespace MauiApp1;

// https://jsonplaceholder.typicode.com/posts
// https://httpbin.org


public partial class MainPage : ContentPage
{
	int page = 0;
	int PokeNum = 0;
	bool forward = true;
	List<string> The_List = new List<string>();
	bool done = false;
	Networks network;

	public MainPage()
	{
		InitializeComponent();
	}


	private void OnRetryBtn (object sender, EventArgs e){
		HttpClient client = new HttpClient();
		string url = "https://httpbin.org/status/500";

		_ = DownloadStringWithRetries(client, url);
	}

	async Task <string> DownloadStringWithRetries(HttpClient client, string url){
		//Retry after 1 second, then after 2 seconds, then 4 seconds...

		TimeSpan nextDelay = TimeSpan.FromSeconds(1);
		string result = "";
		for (int i = 0; i != 3; i++){
			try{
				result = await client.GetStringAsync(url);
				Console.WriteLine(result);
				return result;
			}
			catch(Exception e){
				Console.WriteLine("No jalo xD", e);
			}

			await Task.Delay(nextDelay);
			nextDelay = nextDelay + nextDelay;
		}
		url = "https://httpbin.org/stream/2";
		//Try one last time, allowing the error to progater 
		result = await client.GetStringAsync(url);
		Console.WriteLine(result);
		return result;
	}

	//PROGRESS BAR
	private async void OnFromProgress (object sender, EventArgs e){
		//PBar.Progress = 0;

		Progress<int> progress = new Progress<int> ();
		progress.ProgressChanged += (sender, e) =>{
			Console.WriteLine($"Progress: {e}%");
			//PBar.Progress = Convert.ToDouble(e)/100;
		};
		await ProcessData(progress);
		Console.WriteLine("Process Completed.");
	}

	static async Task ProcessData(IProgress<int> progress){
		for(int i = 0; i<= 100 ; i++){
			//Simulate some long-running operation
			await Task.Delay(500);
			//Report progress 
			progress.Report(i);
		}
	}

	//POKEAPI ACTIVITY
	
	// Function of the first PokeButton
	private void OnPokeBtn (object sender, EventArgs e){
		//initialize the client variable
		HttpClient client = new HttpClient();
		string PokeUrl = "https://pokeapi.co/api/v2/bility/?limit=20";
		_ = DownloadPokeStrings(client, PokeUrl);
	}

	//Function for the next PokeButton
	private void OnNxtPokes (object sender, EventArgs e){
		HttpClient client = new HttpClient();
		page += 20;
		string PokeUrl = "https://pokeapi.co/api/v2/ability/?limit=20&offset="+page;
		forward = true;
		_ = DownloadPokeStrings(client, PokeUrl);
	}

	//Function for the previous PokeButton
	private void OnPrevPokes (object sender, EventArgs e){
		HttpClient client = new HttpClient();
		page -= 20;
		string PokeUrl = "https://pokeapi.co/api/v2/ability/?limit=20&offset="+page;
		forward = false;
		_ = DownloadPokeStrings(client, PokeUrl);
	}

	//Function to download of the API the Pokes
	async Task <string> DownloadPokeStrings(HttpClient client, string PokeUrl){
		
		HttpResponseMessage response = await client.GetAsync(PokeUrl);
		TimeSpan  nextDelay = TimeSpan.FromSeconds(1);
		string result = "";
		for (int i = 1; i != 3; i++){
			try{
				result = await response.Content.ReadAsStringAsync();
				//Convert string to JSON
				PokeData jsonString = JsonSerializer.Deserialize<PokeData>(result);

				Console.WriteLine(jsonString.count);
				Console.WriteLine(jsonString.next);
				Console.WriteLine(jsonString.previous);
				foreach( PokeAbilities poke in jsonString.results){
										if(forward == true){
						PokeNum = PokeNum + 1;
					}
					else if(forward == false){
						PokeNum = PokeNum - 1;
					}
					Console.WriteLine($"Poke #{PokeNum}");
					Console.WriteLine(poke.name);
					Console.WriteLine(poke.url);

				}
				PBar.Progress = Convert.ToDouble(PokeNum*100/367)/100;
				return result;
			}
			catch(Exception e){
				Console.WriteLine("No jalo xD", e);
			}
			await Task.Delay(nextDelay);
			nextDelay = nextDelay + nextDelay;			

		};
		PokeUrl = "https://pokeapi.co/api/v2/ability/?limit=20&offset="+page;
		result = await client.GetStringAsync(PokeUrl);
		Console.WriteLine(result);
		return result;
	}


	public class PokeData {
		public int count {get; set;}
		public string next {get; set;}
		public string previous {get; set;}
		public List<PokeAbilities> results {get; set;}
	}
	public class PokeAbilities {
		public string name {get ; set;}
		public string url {get; set;}
	}

	//PARALLEL PROGRAMMING ACTIVITY
		private void OnRickBtn (object sender, EventArgs e){
			HttpClient client = new HttpClient();
			_ = DownloadRickStrings(client);
		}

		async Task <string> DownloadRickStrings(HttpClient client){

		TimeSpan  nextDelay = TimeSpan.FromSeconds(1);
		string result = "";
		string RickUrl = "";
		string PokeUrl = "";
		for (int i = 1; i != 3; i++){
			try{
				if (done == false) {
					for (int j = 1; j != 4; j++){
						RickUrl = "https://rickandmortyapi.com/api/character/"+j;
						HttpResponseMessage response = await client.GetAsync(RickUrl);
						result = await response.Content.ReadAsStringAsync();
						The_List.Add(result);
					}
					for (int k = 1; k != 4; k++){
						PokeUrl = "https://pokeapi.co/api/v2/berry-flavor/"+k;
						HttpResponseMessage response = await client.GetAsync(PokeUrl);
						result = await response.Content.ReadAsStringAsync();
						The_List.Add(result);					
					}
					done = true;
				}

				Parallel.ForEach(The_List, ruck => {
					Console.WriteLine(ruck);
				});
				return result;
			}
			catch(Exception e){
				Console.WriteLine("No jalo xD", e);
			}
			await Task.Delay(nextDelay);
			nextDelay = nextDelay + nextDelay;			

		};
		RickUrl = "https://rickandmortyapi.com/api/character/2";
		result = await client.GetStringAsync(RickUrl);
		Console.WriteLine(result);
		return result;
	}
	public async Task <string> GetResults (HttpClient Client, String Url){
        string message = await Client.GetStringAsync(Url);
        Console.WriteLine(message);
		Response res = await network.Send("127.0.0.1", 21, message);
        return message;
    }


	private void OnNetworkClicked (object sender, EventArgs e){
		network = new Networks();
		HttpClient client = new HttpClient();
		string RickUrl = "https://rickandmortyapi.com/api/character/1,2,3";
		string PokeUrl = "https://pokeapi.co/api/v2";
		Parallel.Invoke(
			async () => await GetResults(client, RickUrl),
			async () => await GetResults(client, PokeUrl)
		);
	}
}


