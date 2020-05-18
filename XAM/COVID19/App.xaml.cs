using Perimeter.Extensions;
using Perimeter.Interfaces;
using Perimeter.Models;
using Perimeter.ViewModels;
using Perimeter.Views;

using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;

using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Perimeter
{
    public partial class App : Application
    {
        
        public static User CurrentUser { get; set; }
        public static List<Guid> DevicesSeen { get; set; }

        public static bool IsFirstTime { get; set; } = true;

        public static bool IsGATTServiceRunning { get; set; } = false;

        public static string ServiceId = "19536E67-1000-4588-9F3A-5340B671C019";

        
        // read only characteristic
        public const string FirmwareVersion = "19536E67-2000-4588-9F3A-5340B671C019";
        public static string FirmwareVersionValue = "1.0.0.0";

        // read only characteristic
        public const string DeviceNickname = "19536E67-3000-4588-9F3A-5340B671C019";
        public static string DeviceNicknameValue = "";

        // read only characteristic
        public const string IsInfected = "19536E67-4000-4588-9F3A-5340B671C019";
        public static string IsInfectedValue = "false";

        public const string DeviceContact = "19536E67-5000-4588-9F3A-5340B671C019";
        public static string DeviceContactValue = "";

        public const string DeviceInfo = "19536E67-9000-4588-9F3A-5340B671C019";
        public static string DeviceInfoValue = "";

        public const string DeviceMAC = "19536E67-6000-4588-9F3A-5340B671C019";
        public static string DeviceMACValue = "";

        // write characteristic -> exchange data
        public static string InitData = "19536E67-7000-4588-9F3A-5340B671C019";

        public const string DeviceNicknameDescriptor = "10000000-1000-4588-9F3A-5340B671C019";
        public static string DeviceNicknameDescriptorValue = "";


        public const string ManufacturerName = "19536E67-8000-4588-9F3A-5340B671C019";
        public static string ManufacturerNameValue = "Perimeter App";

        public static string AndroidBluetoothDeviceName { get; set; }
      
        public static Xamarin.Forms.Maps.Position DefaultLocation { get; set; }

        public static IGeolocator Locator { get; set; }
        public static double ScreenHeight;
        public static double ScreenWidth;

        private static HttpClient httpClient;
        private static HttpClient httpClientWithTimeout;

        public static string Output { get; private set; }


        public static bool IsAuthenticated
        {
            get
            {
                return false;
            }

        }
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();         
            //MainPage = new ShieldPage();

            Locator = CrossGeolocator.Current;
            DevicesSeen = new List<Guid>();
            List<string> files = GetDataFilesList("*.xml");
            if (files != null && files.Any())
            {
                IsFirstTime = false;
                MainPage = new NavigationPage(new ShieldPage());
            }
            else
            {
                MainPage = new MainPage();
            }
                
        }

        public  async Task LoadSettingsAsync()
        {
            try
            {

                CurrentUser = await GetUser();

                

                if (CurrentUser == null)
                {
                    

                } else
                {
                    App.DeviceMACValue = CurrentUser.Id;
                    App.DeviceNicknameValue = CurrentUser.UserId;
                    App.IsInfectedValue = CurrentUser.IsInfected ? "true" : "false";

                    if(!IsFirstTime)
                    {
                        NavigationPage page = (NavigationPage)Application.Current.MainPage;
                        if(page!=null)
                            MessagingCenter.Send<ShieldPage>((ShieldPage)page.CurrentPage, "ReloadProfile");
                    }
                        
                }

            }
            catch (Exception ex)
            {

            }
        }

        public async static Task<User> GetUser()
        {
            var fileService = DependencyService.Get<ISaveAndLoad>();

            User result = null;

            string localFile = "User.xml";

            try
            {

                if (localFile != null)
                {
                    string localData = await fileService.LoadTextAsync(localFile);

                    result = ObjectSerializer<User>.FromXml((string)localData);
                }

            }
            catch (Exception ex)
            {
                localFile = null;
                //LoggingService.WriteLine(ex.Message, Severities.Error, Targets.Debug);

            }


            return result;

        }

        public async static Task<bool> SetUser(User user)
        {
            var fileService = DependencyService.Get<ISaveAndLoad>();

            bool result = false;

            try
            {
                string data = ObjectSerializer<User>.ToXml(user);

                if (!string.IsNullOrEmpty(data))
                {
                    await fileService.SaveTextAsync("User.xml", data);

                }

                result = true;

            }
            catch (Exception ex)
            {
                //LoggingService.WriteLine(ex.Message, Severities.Error, Targets.Debug);


            }

            return result;
        }



        public static List<string> GetDataFilesList(string filter)
        {
            var fileService = DependencyService.Get<ISaveAndLoad>();

            List<string> result = new List<string>();

            try
            {

                //if (localFile != null)
                {

                    result = fileService.GetAllFiles(filter).ToList();

                }

            }
            catch (Exception ex)
            {
                //LoggingService.WriteLine(ex.Message, Severities.Error, Targets.Debug);

            }


            return result;

        }


        public static void VibrateDevice()
        {
            try
            {
                // Use default vibration length
                Vibration.Vibrate();

                // Or use specified time
                var duration = TimeSpan.FromSeconds(1);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        public static byte[] FromHexString(string hex)
        {
            hex = hex
                .Replace("-", String.Empty)
                .Replace(" ", String.Empty);

            return Enumerable
                .Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }


        public static string ToHexString( byte[] bytes)
            => String.Concat(bytes.Select(b => b.ToString("X2")));
    

        public static Task DisplayAlertAsync(string title, string message, string cancel)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public static Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
        protected override  async void OnStart()
        {
          
            try
            {
                await LoadSettingsAsync();

                App.DeviceInfoValue = Xamarin.Essentials.DeviceInfo.Name;

                StartGattServer();

                var pos = await CrossGeolocator.Current.GetPositionAsync();

                if (pos != null)
                    App.DefaultLocation = new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude);

            }
            catch (Exception ex)
            {

            }

            if(App.DefaultLocation==null)
                App.DefaultLocation= new Xamarin.Forms.Maps.Position(51.500152, -0.126236);

        }

        public static List<Item> GetSIDummyData()
        {
            List<Item> result = new List<Item>();

            Random rnd = new Random(DateTime.Now.Second);

            string[] surnames = new string[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson", "Martinez", "Anderson", "Taylor", "Thomas", "Hernandez", "Moore", "Martin", "Jackson", "Thompson", "White", "Lopez", "Lee", "Gonzalez", "Harris", "Clark", "Lewis", "Robinson", "Walker", "Perez", "Hall", "Young", "Allen", "Sanchez", "Wright", "King", "Scott", "Green", "Baker", "Adams", "Nelson", "Hill", "Ramirez", "Campbell", "Mitchell", "Roberts", "Carter", "Phillips", "Evans", "Turner", "Torres", "Parker", "Collins", "Edwards", "Stewart", "Flores", "Morris", "Nguyen", "Murphy", "Rivera", "Cook", "Rogers", "Morgan", "Peterson", "Cooper", "Reed", "Bailey", "Bell", "Gomez", "Kelly", "Howard", "Ward", "Cox", "Diaz", "Richardson", "Wood", "Watson", "Brooks", "Bennett", "Gray", "James", "Reyes", "Cruz", "Hughes", "Price", "Myers", "Long", "Foster", "Sanders", "Ross", "Morales", "Powell", "Sullivan", "Russell", "Ortiz", "Jenkins", "Gutierrez", "Perry", "Butler", "Barnes", "Fisher", "Henderson", "Coleman", "Simmons", "Patterson", "Jordan", "Reynolds", "Hamilton", "Graham", "Kim", "Gonzales", "Alexander", "Ramos", "Wallace", "Griffin", "West", "Cole", "Hayes", "Chavez", "Gibson", "Bryant", "Ellis", "Stevens", "Murray", "Ford", "Marshall", "Owens", "Mcdonald", "Harrison", "Ruiz", "Kennedy", "Wells", "Alvarez", "Woods", "Mendoza", "Castillo", "Olson", "Webb", "Washington", "Tucker", "Freeman", "Burns", "Henry", "Vasquez", "Snyder", "Simpson", "Crawford", "Jimenez", "Porter", "Mason", "Shaw", "Gordon", "Wagner", "Hunter", "Romero", "Hicks", "Dixon", "Hunt", "Palmer", "Robertson", "Black", "Holmes", "Stone", "Meyer", "Boyd", "Mills", "Warren", "Fox", "Rose", "Rice", "Moreno", "Schmidt", "Patel", "Ferguson", "Nichols", "Herrera", "Medina", "Ryan", "Fernandez", "Weaver", "Daniels", "Stephens", "Gardner", "Payne", "Kelley", "Dunn", "Pierce", "Arnold", "Tran", "Spencer", "Peters", "Hawkins", "Grant", "Hansen", "Castro", "Hoffman", "Hart", "Elliott", "Cunningham", "Knight", "Bradley", "Carroll", "Hudson", "Duncan", "Armstrong", "Berry", "Andrews", "Johnston", "Ray", "Lane", "Riley", "Carpenter", "Perkins", "Aguilar", "Silva", "Richards", "Willis", "Matthews", "Chapman", "Lawrence", "Garza", "Vargas", "Watkins", "Wheeler", "Larson", "Carlson", "Harper", "George", "Greene", "Burke", "Guzman", "Morrison", "Munoz", "Jacobs", "Obrien", "Lawson", "Franklin", "Lynch", "Bishop", "Carr", "Salazar", "Austin", "Mendez", "Gilbert", "Jensen", "Williamson", "Montgomery", "Harvey", "Oliver", "Howell", "Dean", "Hanson", "Weber", "Garrett", "Sims", "Burton", "Fuller", "Soto", "Mccoy", "Welch", "Chen", "Schultz", "Walters", "Reid", "Fields", "Walsh", "Little", "Fowler", "Bowman", "Davidson", "May", "Day", "Schneider", "Newman", "Brewer", "Lucas", "Holland", "Wong", "Banks", "Santos", "Curtis", "Pearson", "Delgado", "Valdez", "Pena", "Rios", "Douglas", "Sandoval", "Barrett", "Hopkins", "Keller", "Guerrero", "Stanley", "Bates", "Alvarado", "Beck", "Ortega", "Wade", "Estrada", "Contreras", "Barnett", "Caldwell", "Santiago", "Lambert", "Powers", "Chambers", "Nunez", "Craig", "Leonard", "Lowe", "Rhodes", "Byrd", "Gregory", "Shelton", "Frazier", "Becker", "Maldonado", "Fleming", "Vega", "Sutton", "Cohen", "Jennings", "Parks", "Mcdaniel", "Watts", "Barker", "Norris", "Vaughn", "Vazquez", "Holt", "Schwartz", "Steele", "Benson", "Neal", "Dominguez", "Horton", "Terry", "Wolfe", "Hale", "Lyons", "Graves", "Haynes", "Miles", "Park", "Warner", "Padilla", "Bush", "Thornton", "Mccarthy", "Mann", "Zimmerman", "Erickson", "Fletcher", "Mckinney", "Page", "Dawson", "Joseph", "Marquez", "Reeves", "Klein", "Espinoza", "Baldwin", "Moran", "Love", "Robbins", "Higgins", "Ball", "Cortez", "Le", "Griffith", "Bowen", "Sharp", "Cummings", "Ramsey", "Hardy", "Swanson", "Barber", "Acosta", "Luna", "Chandler", "Blair", "Daniel", "Cross", "Simon", "Dennis", "Oconnor", "Quinn", "Gross", "Navarro", "Moss", "Fitzgerald", "Doyle", "Mclaughlin", "Rojas", "Rodgers", "Stevenson", "Singh", "Yang", "Figueroa", "Harmon", "Newton", "Paul", "Manning", "Garner", "Mcgee", "Reese", "Francis", "Burgess", "Adkins", "Goodman", "Curry", "Brady", "Christensen", "Potter", "Walton", "Goodwin", "Mullins", "Molina", "Webster", "Fischer", "Campos", "Avila", "Sherman", "Todd", "Chang", "Blake", "Malone", "Wolf", "Hodges", "Juarez", "Gill", "Farmer", "Hines", "Gallagher", "Duran", "Hubbard", "Cannon", "Miranda", "Wang", "Saunders", "Tate", "Mack", "Hammond", "Carrillo", "Townsend", "Wise", "Ingram", "Barton", "Mejia", "Ayala", "Schroeder", "Hampton", "Rowe", "Parsons", "Frank", "Waters", "Strickland", "Osborne", "Maxwell", "Chan", "Deleon", "Norman", "Harrington", "Casey", "Patton", "Logan", "Bowers", "Mueller", "Glover", "Floyd", "Hartman", "Buchanan", "Cobb", "French", "Kramer", "Mccormick", "Clarke", "Tyler", "Gibbs", "Moody", "Conner", "Sparks", "Mcguire", "Leon", "Bauer", "Norton", "Pope", "Flynn", "Hogan", "Robles", "Salinas", "Yates", "Lindsey", "Lloyd", "Marsh", "Mcbride", "Owen", "Solis", "Pham", "Lang", "Pratt", "Lara", "Brock", "Ballard", "Trujillo", "Shaffer", "Drake", "Roman", "Aguirre", "Morton", "Stokes", "Lamb", "Pacheco", "Patrick", "Cochran", "Shepherd", "Cain", "Burnett", "Hess", "Li", "Cervantes", "Olsen", "Briggs", "Ochoa", "Cabrera", "Velasquez", "Montoya", "Roth", "Meyers", "Cardenas", "Fuentes", "Weiss", "Wilkins", "Hoover", "Nicholson", "Underwood", "Short", "Carson", "Morrow", "Colon", "Holloway", "Summers", "Bryan", "Petersen", "Mckenzie", "Serrano", "Wilcox", "Carey", "Clayton", "Poole", "Calderon", "Gallegos", "Greer", "Rivas", "Guerra", "Decker", "Collier", "Wall", "Whitaker", "Bass", "Flowers", "Davenport", "Conley", "Houston", "Huff", "Copeland", "Hood", "Monroe", "Massey", "Roberson", "Combs", "Franco", "Larsen", "Pittman", "Randall", "Skinner", "Wilkinson", "Kirby", "Cameron", "Bridges", "Anthony", "Richard", "Kirk", "Bruce", "Singleton", "Mathis", "Bradford", "Boone", "Abbott", "Charles", "Allison", "Sweeney", "Atkinson", "Horn", "Jefferson", "Rosales", "York", "Christian", "Phelps", "Farrell", "Castaneda", "Nash", "Dickerson", "Bond", "Wyatt", "Foley", "Chase", "Gates", "Vincent", "Mathews", "Hodge", "Garrison", "Trevino", "Villarreal", "Heath", "Dalton", "Valencia", "Callahan", "Hensley", "Atkins", "Huffman", "Roy", "Boyer", "Shields", "Lin", "Hancock", "Grimes", "Glenn", "Cline", "Delacruz", "Camacho", "Dillon", "Parrish", "Oneill", "Melton", "Booth", "Kane", "Berg", "Harrell", "Pitts", "Savage", "Wiggins", "Brennan", "Salas", "Marks", "Russo", "Sawyer", "Baxter", "Golden", "Hutchinson", "Liu", "Walter", "Mcdowell", "Wiley", "Rich", "Humphrey", "Johns", "Koch", "Suarez", "Hobbs", "Beard", "Gilmore", "Ibarra", "Keith", "Macias", "Khan", "Andrade", "Ware", "Stephenson", "Henson", "Wilkerson", "Dyer", "Mcclure", "Blackwell", "Mercado", "Tanner", "Eaton", "Clay", "Barron", "Beasley", "Oneal", "Small", "Preston", "Wu", "Zamora", "Macdonald", "Vance", "Snow", "Mcclain", "Stafford", "Orozco", "Barry", "English", "Shannon", "Kline", "Jacobson", "Woodard", "Huang", "Kemp", "Mosley", "Prince", "Merritt", "Hurst", "Villanueva", "Roach", "Nolan", "Lam", "Yoder", "Mccullough", "Lester", "Santana", "Valenzuela", "Winters", "Barrera", "Orr", "Leach", "Berger", "Mckee", "Strong", "Conway", "Stein", "Whitehead", "Bullock", "Escobar", "Knox", "Meadows", "Solomon", "Velez", "Odonnell", "Kerr", "Stout", "Blankenship", "Browning", "Kent", "Lozano", "Bartlett", "Pruitt", "Buck", "Barr", "Gaines", "Durham", "Gentry", "Mcintyre", "Sloan", "Rocha", "Melendez", "Herman", "Sexton", "Moon", "Hendricks", "Rangel", "Stark", "Lowery", "Hardin", "Hull", "Sellers", "Ellison", "Calhoun", "Gillespie", "Mora", "Knapp", "Mccall", "Morse", "Dorsey", "Weeks", "Nielsen", "Livingston", "Leblanc", "Mclean", "Bradshaw", "Glass", "Middleton", "Buckley", "Schaefer", "Frost", "Howe", "House", "Mcintosh", "Ho", "Pennington", "Reilly", "Hebert", "Mcfarland", "Hickman", "Noble", "Spears", "Conrad", "Arias", "Galvan", "Velazquez", "Huynh", "Frederick", "Randolph", "Cantu", "Fitzpatrick", "Mahoney", "Peck", "Villa", "Michael", "Donovan", "Mcconnell", "Walls", "Boyle", "Mayer", "Zuniga", "Giles", "Pineda", "Pace", "Hurley", "Mays", "Mcmillan", "Crosby", "Ayers", "Case", "Bentley", "Shepard", "Everett", "Pugh", "David", "Mcmahon", "Dunlap", "Bender", "Hahn", "Harding", "Acevedo", "Raymond", "Blackburn", "Duffy", "Landry", "Dougherty", "Bautista", "Shah", "Potts", "Arroyo", "Valentine", "Meza", "Gould", "Vaughan", "Fry", "Rush", "Avery", "Herring", "Dodson", "Clements", "Sampson", "Tapia", "Bean", "Lynn", "Crane", "Farley", "Cisneros", "Benton", "Ashley", "Mckay", "Finley", "Best", "Blevins", "Friedman", "Moses", "Sosa", "Blanchard", "Huber", "Frye", "Krueger", "Bernard", "Rosario", "Rubio", "Mullen", "Benjamin", "Haley", "Chung", "Moyer", "Choi", "Horne", "Yu", "Woodward", "Ali", "Nixon", "Hayden", "Rivers", "Estes", "Mccarty", "Richmond", "Stuart", "Maynard", "Brandt", "Oconnell", "Hanna", "Sanford", "Sheppard", "Church", "Burch", "Levy", "Rasmussen", "Coffey", "Ponce", "Faulkner", "Donaldson", "Schmitt", "Novak", "Costa", "Montes", "Booker", "Cordova", "Waller", "Arellano", "Maddox", "Mata", "Bonilla", "Stanton", "Compton", "Kaufman", "Dudley", "Mcpherson", "Beltran", "Dickson", "Mccann", "Villegas", "Proctor", "Hester", "Cantrell", "Daugherty", "Cherry", "Bray", "Davila", "Rowland", "Madden", "Levine", "Spence", "Good", "Irwin", "Werner", "Krause", "Petty", "Whitney", "Baird", "Hooper", "Pollard", "Zavala", "Jarvis", "Holden", "Haas", "Hendrix", "Mcgrath", "Bird", "Lucero", "Terrell", "Riggs", "Joyce", "Mercer", "Rollins", "Galloway", "Duke", "Odom", "Andersen", "Downs", "Hatfield", "Benitez", "Archer", "Huerta", "Travis", "Mcneil", "Hinton", "Zhang", "Hays", "Mayo", "Fritz", "Branch", "Mooney", "Ewing", "Ritter", "Esparza", "Frey", "Braun", "Gay", "Riddle", "Haney", "Kaiser", "Holder", "Chaney", "Mcknight", "Gamble", "Vang", "Cooley", "Carney", "Cowan", "Forbes", "Ferrell", "Davies", "Barajas", "Shea", "Osborn", "Bright", "Cuevas", "Bolton", "Murillo", "Lutz", "Duarte", "Kidd", "Key", "Cooke" };
            string[] male = new string[] { "James", "John", "Robert", "Michael", "William", "David", "Richard", "Charles", "Joseph", "Thomas", "Christopher", "Daniel", "Paul", "Mark", "Donald", "George", "Kenneth", "Steven", "Edward", "Brian", "Ronald", "Anthony", "Kevin", "Jason", "Matthew", "Gary", "Timothy", "Jose", "Larry", "Jeffrey", "Frank", "Scott", "Eric", "Stephen", "Andrew", "Raymond", "Gregory", "Joshua", "Jerry", "Dennis", "Walter", "Patrick", "Peter", "Harold", "Douglas", "Henry", "Carl", "Arthur", "Ryan", "Roger", "Joe", "Juan", "Jack", "Albert", "Jonathan", "Justin", "Terry", "Gerald", "Keith", "Samuel", "Willie", "Ralph", "Lawrence", "Nicholas", "Roy", "Benjamin", "Bruce", "Brandon", "Adam", "Harry", "Fred", "Wayne", "Billy", "Steve", "Louis", "Jeremy", "Aaron", "Randy", "Howard", "Eugene", "Carlos", "Russell", "Bobby", "Victor", "Martin", "Ernest", "Phillip", "Todd", "Jesse", "Craig", "Alan", "Shawn", "Clarence", "Sean", "Philip", "Chris", "Johnny", "Earl", "Jimmy", "Antonio", "Danny", "Bryan", "Tony", "Luis", "Mike", "Stanley", "Leonard", "Nathan", "Dale", "Manuel", "Rodney", "Curtis", "Norman", "Allen", "Marvin", "Vincent", "Glenn", "Jeffery", "Travis", "Jeff", "Chad", "Jacob", "Lee", "Melvin", "Alfred", "Kyle", "Francis", "Bradley", "Jesus", "Herbert", "Frederick", "Ray", "Joel", "Edwin", "Don", "Eddie", "Ricky", "Troy", "Randall", "Barry", "Alexander", "Bernard", "Mario", "Leroy", "Francisco", "Marcus", "Micheal", "Theodore", "Clifford", "Miguel", "Oscar", "Jay", "Jim", "Tom", "Calvin", "Alex", "Jon", "Ronnie", "Bill", "Lloyd", "Tommy", "Leon", "Derek", "Warren", "Darrell", "Jerome", "Floyd", "Leo", "Alvin", "Tim", "Wesley", "Gordon", "Dean", "Greg", "Jorge", "Dustin", "Pedro", "Derrick", "Dan", "Lewis", "Zachary", "Corey", "Herman", "Maurice", "Vernon", "Roberto", "Clyde", "Glen", "Hector", "Shane", "Ricardo", "Sam", "Rick", "Lester", "Brent", "Ramon", "Charlie", "Tyler", "Gilbert", "Gene", "Marc", "Reginald", "Ruben", "Brett", "Angel", "Nathaniel", "Rafael", "Leslie", "Edgar", "Milton", "Raul", "Ben", "Chester", "Cecil", "Duane", "Franklin", "Andre", "Elmer", "Brad", "Gabriel", "Ron", "Mitchell", "Roland", "Arnold", "Harvey", "Jared", "Adrian", "Karl", "Cory", "Claude", "Erik", "Darryl", "Jamie", "Neil", "Jessie", "Christian", "Javier", "Fernando", "Clinton", "Ted", "Mathew", "Tyrone", "Darren", "Lonnie", "Lance", "Cody", "Julio", "Kelly", "Kurt", "Allan", "Nelson", "Guy", "Clayton", "Hugh", "Max", "Dwayne", "Dwight", "Armando", "Felix", "Jimmie", "Everett", "Jordan", "Ian", "Wallace", "Ken", "Bob", "Jaime", "Casey", "Alfredo", "Alberto", "Dave", "Ivan", "Johnnie", "Sidney", "Byron", "Julian", "Isaac", "Morris", "Clifton", "Willard", "Daryl", "Ross", "Virgil", "Andy", "Marshall", "Salvador", "Perry", "Kirk", "Sergio", "Marion", "Tracy", "Seth", "Kent", "Terrance", "Rene", "Eduardo", "Terrence", "Enrique", "Freddie", "Wade" };
            string[] female = new string[] { "Mary", "Patricia", "Linda", "Barbara", "Elizabeth", "Jennifer", "Maria", "Susan", "Margaret", "Dorothy", "Lisa", "Nancy", "Karen", "Betty", "Helen", "Sandra", "Donna", "Carol", "Ruth", "Sharon", "Michelle", "Laura", "Sarah", "Kimberly", "Deborah", "Jessica", "Shirley", "Cynthia", "Angela", "Melissa", "Brenda", "Amy", "Anna", "Rebecca", "Virginia", "Kathleen", "Pamela", "Martha", "Debra", "Amanda", "Stephanie", "Carolyn", "Christine", "Marie", "Janet", "Catherine", "Frances", "Ann", "Joyce", "Diane", "Alice", "Julie", "Heather", "Teresa", "Doris", "Gloria", "Evelyn", "Jean", "Cheryl", "Mildred", "Katherine", "Joan", "Ashley", "Judith", "Rose", "Janice", "Kelly", "Nicole", "Judy", "Christina", "Kathy", "Theresa", "Beverly", "Denise", "Tammy", "Irene", "Jane", "Lori", "Rachel", "Marilyn", "Andrea", "Kathryn", "Louise", "Sara", "Anne", "Jacqueline", "Wanda", "Bonnie", "Julia", "Ruby", "Lois", "Tina", "Phyllis", "Norma", "Paula", "Diana", "Annie", "Lillian", "Emily", "Robin", "Peggy", "Crystal", "Gladys", "Rita", "Dawn", "Connie", "Florence", "Tracy", "Edna", "Tiffany", "Carmen", "Rosa", "Cindy", "Grace", "Wendy", "Victoria", "Edith", "Kim", "Sherry", "Sylvia", "Josephine", "Thelma", "Shannon", "Sheila", "Ethel", "Ellen", "Elaine", "Marjorie", "Carrie", "Charlotte", "Monica", "Esther", "Pauline", "Emma", "Juanita", "Anita", "Rhonda", "Hazel", "Amber", "Eva", "Debbie", "April", "Leslie", "Clara", "Lucille", "Jamie", "Joanne", "Eleanor", "Valerie", "Danielle", "Megan", "Alicia", "Suzanne", "Michele", "Gail", "Bertha", "Darlene", "Veronica", "Jill", "Erin", "Geraldine", "Lauren", "Cathy", "Joann", "Lorraine", "Lynn", "Sally", "Regina", "Erica", "Beatrice", "Dolores", "Bernice", "Audrey", "Yvonne", "Annette", "June", "Samantha", "Marion", "Dana", "Stacy", "Ana", "Renee", "Ida", "Vivian", "Roberta", "Holly", "Brittany", "Melanie", "Loretta", "Yolanda", "Jeanette", "Laurie", "Katie", "Kristen", "Vanessa", "Alma", "Sue", "Elsie", "Beth", "Jeanne", "Vicki", "Carla", "Tara", "Rosemary", "Eileen", "Terri", "Gertrude", "Lucy", "Tonya", "Ella", "Stacey", "Wilma", "Gina", "Kristin", "Jessie", "Natalie", "Agnes", "Vera", "Willie", "Charlene", "Bessie", "Delores", "Melinda", "Pearl", "Arlene", "Maureen", "Colleen", "Allison", "Tamara", "Joy", "Georgia", "Constance", "Lillie", "Claudia", "Jackie", "Marcia", "Tanya", "Nellie", "Minnie", "Marlene", "Heidi", "Glenda", "Lydia", "Viola", "Courtney", "Marian", "Stella", "Caroline", "Dora", "Jo", "Vickie", "Mattie", "Terry", "Maxine", "Irma", "Mabel", "Marsha", "Myrtle", "Lena", "Christy", "Deanna", "Patsy", "Hilda", "Gwendolyn", "Jennie", "Nora", "Margie", "Nina", "Cassandra", "Leah", "Penny", "Kay", "Priscilla", "Naomi", "Carole", "Brandy", "Olga", "Billie", "Dianne", "Tracey", "Leona", "Jenny", "Felicia", "Sonia", "Miriam", "Velma", "Becky", "Bobbie", "Violet", "Kristina", "Toni", "Misty", "Mae", "Shelly", "Daisy", "Ramona", "Sherri", "Erika", "Katrina", "Claire", "Lindsey", "Lindsay", "Geneva", "Guadalupe", "Belinda", "Margarita", "Sheryl", "Cora", "Faye", "Ada", "Natasha", "Sabrina", "Isabel", "Marguerite", "Hattie", "Harriet", "Molly", "Cecilia", "Kristi", "Brandi", "Blanche", "Sandy", "Rosie", "Joanna", "Iris", "Eunice", "Angie", "Inez", "Lynda", "Madeline", "Amelia", "Alberta", "Genevieve", "Monique", "Jodi", "Janie", "Maggie", "Kayla", "Sonya", "Jan", "Lee", "Kristine", "Candace", "Fannie", "Maryann", "Opal", "Alison", "Yvette", "Melody", "Luz", "Susie", "Olivia", "Flora", "Shelley", "Kristy", "Mamie", "Lula", "Lola", "Verna", "Beulah", "Antoinette", "Candice", "Juana", "Jeannette", "Pam", "Kelli", "Hannah", "Whitney", "Bridget", "Karla", "Celia", "Latoya", "Patty", "Shelia", "Gayle", "Della", "Vicky", "Lynne", "Sheri", "Marianne", "Kara", "Jacquelyn", "Erma", "Blanca", "Myra", "Leticia", "Pat", "Krista", "Roxanne", "Angelica", "Johnnie", "Robyn", "Francis", "Adrienne", "Rosalie", "Alexandra", "Brooke", "Bethany", "Sadie", "Bernadette", "Traci", "Jody", "Kendra", "Jasmine", "Nichole", "Rachael", "Chelsea", "Mable", "Ernestine", "Muriel", "Marcella", "Elena", "Krystal", "Angelina", "Nadine", "Kari", "Estelle", "Dianna", "Paulette", "Lora", "Mona", "Doreen", "Rosemarie", "Angel", "Desiree", "Antonia", "Hope", "Ginger", "Janis", "Betsy", "Christie", "Freda", "Mercedes", "Meredith", "Lynette", "Teri", "Cristina", "Eula", "Leigh", "Meghan", "Sophia", "Eloise", "Rochelle", "Gretchen", "Cecelia", "Raquel", "Henrietta", "Alyssa", "Jana", "Kelley", "Gwen", "Kerry", "Jenna", "Tricia", "Laverne", "Olive", "Alexis", "Tasha", "Silvia", "Elvira", "Casey", "Delia", "Sophie", "Kate", "Patti", "Lorena", "Kellie", "Sonja", "Lila", "Lana", "Darla", "May", "Mindy", "Essie", "Mandy", "Lorene", "Elsa", "Josefina", "Jeannie", "Miranda", "Dixie", "Lucia", "Marta", "Faith", "Lela", "Johanna", "Shari", "Camille", "Tami", "Shawna", "Elisa", "Ebony", "Melba", "Ora", "Nettie", "Tabitha", "Ollie", "Jaime", "Winifred", "Kristie", "Marina", "Alisha", "Aimee", "Rena", "Myrna", "Marla", "Tammie", "Latasha", "Bonita", "Patrice", "Ronda", "Sherrie", "Addie", "Francine", "Deloris", "Stacie", "Adriana", "Cheri", "Shelby", "Abigail", "Celeste", "Jewel", "Cara", "Adele", "Rebekah", "Lucinda", "Dorthy", "Chris", "Effie", "Trina", "Reba", "Shawn", "Sallie", "Aurora", "Lenora", "Etta", "Lottie", "Kerri", "Trisha", "Nikki", "Estella", "Francisca", "Josie", "Tracie", "Marissa", "Karin", "Brittney", "Janelle", "Lourdes", "Laurel", "Helene", "Fern", "Elva", "Corinne", "Kelsey", "Ina", "Bettie", "Elisabeth", "Aida", "Caitlin", "Ingrid", "Iva", "Eugenia", "Christa", "Goldie", "Cassie", "Maude", "Jenifer", "Therese", "Frankie", "Dena", "Lorna", "Janette", "Latonya", "Candy", "Morgan", "Consuelo", "Tamika", "Rosetta", "Debora", "Cherie", "Polly", "Dina", "Jewell", "Fay", "Jillian", "Dorothea", "Nell", "Trudy", "Esperanza", "Patrica", "Kimberley", "Shanna", "Helena", "Carolina", "Cleo", "Stefanie", "Rosario", "Ola", "Janine", "Mollie", "Lupe", "Alisa", "Lou", "Maribel", "Susanne", "Bette", "Susana", "Elise", "Cecile", "Isabelle", "Lesley", "Jocelyn", "Paige", "Joni", "Rachelle", "Leola", "Daphne", "Alta", "Ester", "Petra", "Graciela", "Imogene", "Jolene", "Keisha", "Lacey", "Glenna", "Gabriela", "Keri", "Ursula", "Lizzie", "Kirsten", "Shana", "Adeline", "Mayra", "Jayne", "Jaclyn", "Gracie", "Sondra", "Carmela", "Marisa", "Rosalind", "Charity", "Tonia", "Beatriz", "Marisol", "Clarice", "Jeanine", "Sheena", "Angeline", "Frieda", "Lily", "Robbie", "Shauna", "Millie", "Claudette", "Cathleen", "Angelia", "Gabrielle", "Autumn", "Katharine", "Summer", "Jodie", "Staci", "Lea", "Christi", "Jimmie", "Justine", "Elma", "Luella", "Margret", "Dominique", "Socorro", "Rene", "Martina", "Margo", "Mavis", "Callie", "Bobbi", "Maritza", "Lucile", "Leanne", "Jeannine", "Deana", "Aileen", "Lorie", "Ladonna", "Willa", "Manuela", "Gale", "Selma", "Dolly", "Sybil", "Abby", "Lara", "Dale", "Ivy", "Dee", "Winnie", "Marcy", "Luisa", "Jeri", "Magdalena", "Ofelia", "Meagan", "Audra", "Matilda", "Leila", "Cornelia", "Bianca", "Simone", "Bettye", "Randi", "Virgie", "Latisha", "Barbra", "Georgina", "Eliza", "Leann", "Bridgette", "Rhoda", "Haley", "Adela", "Nola", "Bernadine", "Flossie", "Ila", "Greta", "Ruthie", "Nelda", "Minerva", "Lilly", "Terrie", "Letha", "Hilary", "Estela", "Valarie", "Brianna", "Rosalyn", "Earline", "Catalina", "Ava", "Mia", "Clarissa", "Lidia", "Corrine", "Alexandria", "Concepcion", "Tia", "Sharron", "Rae", "Dona", "Ericka", "Jami", "Elnora", "Chandra", "Lenore", "Neva", "Marylou", "Melisa", "Tabatha", "Serena", "Avis", "Allie", "Sofia", "Jeanie", "Odessa", "Nannie", "Harriett", "Loraine", "Penelope", "Milagros", "Emilia", "Benita", "Allyson", "Ashlee", "Tania", "Tommie", "Esmeralda", "Karina", "Eve", "Pearlie", "Zelma", "Malinda", "Noreen", "Tameka", "Saundra", "Hillary", "Amie", "Althea", "Rosalinda", "Jordan", "Lilia", "Alana", "Gay", "Clare", "Alejandra", "Elinor", "Michael", "Lorrie", "Jerri", "Darcy", "Earnestine", "Carmella", "Taylor", "Noemi", "Marcie", "Liza", "Annabelle", "Louisa", "Earlene", "Mallory", "Carlene", "Nita", "Selena", "Tanisha", "Katy", "Julianne", "John", "Lakisha", "Edwina", "Maricela", "Margery", "Kenya", "Dollie", "Roxie", "Roslyn", "Kathrine", "Nanette", "Charmaine", "Lavonne", "Ilene", "Kris", "Tammi", "Suzette", "Corine", "Kaye", "Jerry", "Merle", "Chrystal", "Lina", "Deanne", "Lilian", "Juliana", "Aline", "Luann", "Kasey", "Maryanne", "Evangeline", "Colette", "Melva", "Lawanda", "Yesenia", "Nadia", "Madge", "Kathie", "Eddie", "Ophelia", "Valeria", "Nona", "Mitzi", "Mari", "Georgette", "Claudine", "Fran", "Alissa", "Roseann", "Lakeisha", "Susanna", "Reva", "Deidre", "Chasity", "Sheree", "Carly", "James", "Elvia", "Alyce", "Deirdre", "Gena", "Briana", "Araceli", "Katelyn", "Rosanne", "Wendi", "Tessa", "Berta", "Marva", "Imelda", "Marietta", "Marci", "Leonor", "Arline", "Sasha", "Madelyn", "Janna", "Juliette", "Deena", "Aurelia", "Josefa", "Augusta", "Liliana", "Young", "Christian", "Lessie", "Amalia", "Savannah", "Anastasia", "Vilma", "Natalia", "Rosella", "Lynnette", "Corina", "Alfreda", "Leanna", "Carey", "Amparo", "Coleen", "Tamra", "Aisha", "Wilda", "Karyn", "Cherry", "Queen", "Maura", "Mai", "Evangelina", "Rosanna", "Hallie", "Erna", "Enid", "Mariana", "Lacy", "Juliet", "Jacklyn", "Freida", "Madeleine", "Mara", "Hester", "Cathryn", "Lelia", "Casandra", "Bridgett", "Angelita", "Jannie", "Dionne", "Annmarie", "Katina", "Beryl", "Phoebe", "Millicent", "Katheryn", "Diann", "Carissa", "Maryellen", "Liz", "Lauri", "Helga", "Gilda", "Adrian", "Rhea", "Marquita", "Hollie", "Tisha", "Tamera", "Angelique", "Francesca", "Britney", "Kaitlin", "Lolita", "Florine", "Rowena", "Reyna", "Twila", "Fanny", "Janell", "Ines", "Concetta", "Bertie", "Alba", "Brigitte", "Alyson", "Vonda", "Pansy", "Elba", "Noelle", "Letitia", "Kitty", "Deann", "Brandie", "Louella", "Leta", "Felecia", "Sharlene", "Lesa", "Beverley", "Robert", "Isabella", "Herminia", "Terra", "Celina" };

            for(int i=0;i<20;i++)
            {
                string surname = surnames[rnd.Next(surnames.Length)].Substring(0,1);
                string forename = i % 2 == 0 ? male[rnd.Next(male.Length)] : female[rnd.Next(female.Length)];
                string nick = string.Format("{0}{1}",forename,surname);
                result.Add(new Item() { Nickname = nick, RSSI = rnd.Next(3) == 1 ? -47 : -60, Infected = rnd.Next(5) == 1 ? true : false,
                    DateUpdated = DateTime.Now.AddDays(-1).AddHours(rnd.Next(10)).AddMinutes(rnd.Next(59)), DeviceName = rnd.Next(3) == 1 ? "Apple" : "Android"}) ;
            }


            return result;
        }

        public async static Task<List<Item>> GetSIHistory()
        {
            var fileService = DependencyService.Get<ISaveAndLoad>();

            List<Item> result = null;

            string localFile = "SIData.xml";

            try
            {


                if (localFile != null)
                {
                    string localData = await fileService.LoadTextAsync(localFile);

                    result = ObjectSerializer<List<Item>>.FromXml((string)localData);
                }

            }
            catch (Exception ex)
            {
                localFile = null;
                //LoggingService.WriteLine(ex.Message, Severities.Error, Targets.Debug);

            }


            return result;

        }

        public async static Task<bool> SetSIHistory(List<Item> siList)
        {
            var fileService = DependencyService.Get<ISaveAndLoad>();

            bool result = false;

            try
            {
                string data = ObjectSerializer<List<Item>>.ToXml(siList);

                if (!string.IsNullOrEmpty(data))
                {
                    await fileService.SaveTextAsync("SIData.xml", data);
                }

                result = true;

            }
            catch (Exception ex)
            {
                //LoggingService.WriteLine(ex.Message, Severities.Error, Targets.Debug);


            }

            return result;
        }


        protected override void OnSleep()
        {
          
           
        }

        protected override async void OnResume()
        {
            await LoadSettingsAsync();

        }

        public static string GetCharacteristicValue(string guid)
        {
            string result = "";

            try
            {
                switch (guid.ToUpper())
                {
                    case App.ManufacturerName:
                        result = App.ManufacturerNameValue;
                        break;
                    case App.DeviceContact:
                        result = App.DeviceContactValue;
                        break;
                    case App.DeviceInfo:
                        result = App.DeviceInfoValue;
                        break;
                    case App.DeviceNickname:
                        result = App.DeviceNicknameValue;
                        break;
                    case App.FirmwareVersion:
                        result = App.FirmwareVersionValue;
                        break;
                    case App.IsInfected:
                        result = App.IsInfectedValue;
                        break;

                    default:

                        break;
                }
            }
            catch (Exception ex)
            {

            }

            return result;

        }
        public static string GetMACAddress(object device)
        {
            string result = "";

            try
            {
                var settings = DependencyService.Get<ISetttings>();
                result=settings.GetMACAdress(device);

            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public static void OpenBluetoothSettingsAsync()
        {
            try
            {
                var settings = DependencyService.Get<ISetttings>();
                settings.StartBluetoothSettings();

            }
            catch (Exception ex)
            {

            }
        }

        public static void OpenLocationSettingsAsync()
        {
            try
            {
                var settings = DependencyService.Get<ISetttings>();
                settings.StartLocationSettings();

            }
            catch (Exception ex)
            {

            }
        }

        public static void StartGattServer()
        {
            try
            {
                var settings = DependencyService.Get<ISetttings>();

                settings.StartGattServer();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
