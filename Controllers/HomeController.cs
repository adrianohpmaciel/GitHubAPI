using GitHubAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace GitHubAPI.Controllers
{
    public class HomeController : Controller
    {
        public string url = "https://api.github.com/";
        public string pathAPI = string.Empty;
        public static string favoritePath = @"C:\FavoritesGitHubAPIREST\favorites.txt";

        public ActionResult Index()
        {
            Thread.Sleep(200);

            pathAPI = "users/adrianohpmaciel";
            UserData userData = new UserData();

            #region OBTENDO DADOS DO USUÁRIO
            try
            {
                userData = RequestAPI<UserData>(url, pathAPI, "GET");
            }
            catch (Exception e)
            {
                ViewBag.errorMessage = e;
            }

            ViewBag.login = userData.Login;
            ViewBag.name = userData.Name;
            ViewBag.avatar_url = userData.Avatar_url;
            ViewBag.company = userData.Company;
            ViewBag.following = userData.Folllowing;
            ViewBag.followers = userData.Followers;
            ViewBag.publicRepos = userData.Public_Repos;
            ViewBag.created_At = userData.Created_At;
            ViewBag.updated_At = userData.Updated_At;
            #endregion


            pathAPI = "users/adrianohpmaciel/repos";

            #region OBTENDO REPOSITÓRIOS DO USUÁRIO

            RepositoryData[] repositoriesData = RequestAPI<RepositoryData[]>(url, pathAPI, "GET");            

            ViewBag.repositories = repositoriesData;
            ViewBag.Message = "Repositórios públicos";
            #endregion

            ViewBag.Message = "Your repositories page.";

            return View();
        }

        public ActionResult Search(string query = null)
        {
            Thread.Sleep(200);

            #region VERIFICA SE HOUVE ALGO PESQUISADO
            string currentQuery = query;

            if (String.IsNullOrEmpty(query))
            {
                return View();
            }
            #endregion

            pathAPI = "search/repositories?q=" + currentQuery + "&per_page=25&page=1";

            #region REALIZA CONSULTA
            RepositorySearchData searchRepositories = RequestAPI<RepositorySearchData>(url, pathAPI, "GET");
            if (searchRepositories != null) searchRepositories.GetQttPages(15);

            ViewBag.searchRepos = searchRepositories.Items;
            ViewBag.qttPages = searchRepositories.QttPages;
            #endregion

            return View("Search");
        }
        
        public ActionResult Favorites(string addFavUrl = null, string removeFavUrl = null)
        {

            List<string> urlFavorites = GetFavorites();
            List<RepositoryData> favorites = new List<RepositoryData>();

            if (!ChecksFavFile()) CreatesFavFile();
            if (addFavUrl != null) AddFavoriteInFile(addFavUrl);
            if (removeFavUrl != null) RemoveFavorite(removeFavUrl);
            if (addFavUrl != null || removeFavUrl != null) urlFavorites = GetFavorites();

            foreach (string url in urlFavorites)
            {
                Thread.Sleep(200);
                if (!string.IsNullOrEmpty(url)) favorites.Add(RequestAPI<RepositoryData>(url, "", "GET"));
            }

            ViewBag.favorites = favorites;
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Details(string urlRepos)
        {
            Thread.Sleep(200);

            RepositoryData repository = RequestAPI<RepositoryData>(urlRepos, "", "GET");

            Thread.Sleep(200);

            UserData[] contributors = RequestAPI<UserData[]>(repository.Contributors_url, "", "GET");

            ViewBag.urlRepos = urlRepos;
            ViewBag.name = repository.Name;
            ViewBag.description = repository.Description;
            ViewBag.language = repository.Language;
            ViewBag.updated_at = repository.Updated_At;
            ViewBag.owner = repository.Owner;
            ViewBag.contributors = contributors;

            return View();
        }

        public static T RequestAPI<T>(string url, string methodPath, string methodHTTP)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + methodPath);
            request.Method = methodHTTP;
            request.ContentType = "application/json";
            request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 65.0.3325.181 Safari / 537.36 OPR / 52.0.2871.99";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                T result = JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
                return result;
            }
        }
    
        public static bool ChecksFavFile()
        {           

            if (System.IO.File.Exists(favoritePath)) return true;
            else return false;

        }
    
        public static void CreatesFavFile()
        {
            string path = favoritePath.Replace(@"\favorites.txt", @"\");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            System.IO.File.WriteAllText(favoritePath, string.Empty);
        }

        public static bool ExistsFavorite(string favUrl)
        {
            string favscontents = string.Empty;

            using (StreamReader favs = new StreamReader(favoritePath))
            {
                favscontents = favs.ReadToEnd();
                favs.Close();
            }
            
            return favscontents.Contains(favUrl);

        }
       
        public static void AddFavoriteInFile(string favUrl)
        {
           List<string> contents = null;

            if (!ExistsFavorite(favUrl))
            {
                contents = GetFavorites();

                using (StreamWriter favs = new StreamWriter(favoritePath))
                {
                    contents = contents.Where(s => s != "\n" && s != "\r" && !string.IsNullOrEmpty(s)).ToList<string>();
                    favs.Close();
                }
                contents.Add(favUrl);

                System.IO.File.WriteAllLines(favoritePath, contents);
            }
        }

        public static void RemoveFavorite(string favUrl)
        {
            List<string> contents = null;

            if (ExistsFavorite(favUrl))
            {
                contents = GetFavorites();
                contents = contents.Where(s => !s.Contains(favUrl) && !string.IsNullOrEmpty(s)).ToList<string>();                 

                System.IO.File.WriteAllLines(favoritePath, contents);
            }
        }
    
        public static List<string> GetFavorites()
        {
            List<string> contentss = null;
            List<string> newcontentss = new List<string>();

            using (StreamReader favs = new StreamReader(favoritePath))
            {
                contentss = favs.ReadToEnd().Split('\r').ToList<string>();
                favs.Close();
            }
            foreach (string url in contentss)
            {
                newcontentss.Add(url.Replace("\n", "") );
            }

            return newcontentss;
        }
    }
}