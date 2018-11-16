using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Inverted_Index_Campbell_Reid
{
    public partial class MainWindow : Window
    {

        ConcurrentDictionary<string, ConcurrentDictionary<int, List<int>>> invertedIndex = new ConcurrentDictionary<string, ConcurrentDictionary<int, List<int>>>();//Overall index data structure
        string[] files; //Array of file paths
        object lock1 = new object();
        int counter;
        bool? isStemming;
        bool? synonymsOn;
        int searchType; //1 = AND, 2 = OR, 3 = XOR

        public MainWindow()
        {
            InitializeComponent();
        }

        public List<string> GetStopWords()
        {
            List<string> stopWords = new List<string>();

            StreamReader swr = new StreamReader(@"stopwords.txt");
            while (!swr.EndOfStream)
            {
                stopWords.Add(swr.ReadLine()); //Get all stop words, form a list
            }

            return stopWords;
        }

        public void exploreFolder(object root)
        {
            string r = root.ToString();

            Stopwatch t = new Stopwatch();
            t.Start();
            counter = 0;
            List<string> stopWords = GetStopWords();
            string[] stop = stopWords.ToArray();
            files = Directory.GetFiles(r, "*", SearchOption.AllDirectories); //Get all files below the selected folder
            try
            {
                Parallel.For(0, files.Length, new ParallelOptions { MaxDegreeOfParallelism = 10 }, x =>
                {
                    parseFile(x, stop); //Parse all files using multithreading
                });
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            GC.Collect();

            Console.WriteLine("File Index Complete. Total number of unique entries: " + invertedIndex.Count);
            Console.WriteLine("                     Total number of entries: " + counter);

            t.Stop();
            Console.WriteLine("Time taken to index: " + t.Elapsed);
        }

        public void parseFile(object file, string[] stopWords)
        {
            int f;
            int.TryParse(file.ToString(), out f); //Get file ID

            FileStream filestream = new FileStream(files[f], FileMode.Open, FileAccess.Read);
            StreamReader streamreader = new StreamReader(filestream);
            string whole = streamreader.ReadToEnd();                    //Get file as string
            PorterStemmer stemmer = new PorterStemmer();
            streamreader.Close();
            filestream.Close();

            whole = whole.ToLower();
            Regex rgx = new Regex(@"\r\n", RegexOptions.ECMAScript); //Remove special chars and change string to a single line
            whole = rgx.Replace(whole, " ");
            rgx = new Regex(@"[^0-9a-z ]+", RegexOptions.ECMAScript);
            whole = rgx.Replace(whole, "");

            List<string> words = whole.Split(' ').ToList<string>();
            words = words.Where(e => !stopWords.Any(g => g == e)).ToList<string>();

            for (int i = 0; i < words.Count; i++)
            {
                lock (lock1) if(isStemming ?? false) words[i] = stemmer.StemWord(words[i]);                                                         //If stemming is turned on, stem the current word
                ConcurrentDictionary<int, List<int>> indexEntry = invertedIndex.GetOrAdd(words[i], new ConcurrentDictionary<int, List<int>>());     //If the word is already in the index; get it, otherwise; add it
                indexEntry.GetOrAdd(f, new List<int>()).Add(i);                                                                                     //If that word has already occurred in this file; get the occurance list, otherwise; create the list
                lock (lock1) counter++;                                                                                                             //Non essential counter for metrics
            }
        }

        private void selectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd1 = new FolderBrowserDialog();               //user chooses a folder
            fbd1.ShowDialog();
            fbd1.ShowNewFolderButton = false;
            invertedIndex.Clear();                                              //clear the old hashtable
            lock (lock1) isStemming = stemmingBox.IsChecked;
            rootFolderDisplay.Text = fbd1.SelectedPath;
            ParameterizedThreadStart ts = new ParameterizedThreadStart(exploreFolder);
            Thread th = new Thread(ts);                                         //Start the indexing in a seperate thread, so the UI thread is not interrupted
            th.Start(fbd1.SelectedPath);            
        }

        public void searchIndex(object fq)
        {
            Stopwatch s = new Stopwatch();
            PorterStemmer stemmer = new PorterStemmer();
            s.Start();
            string fullQuery = fq.ToString();
            fullQuery = fullQuery.ToLower();
            Regex rgx = new Regex(@"\r\n", RegexOptions.ECMAScript); //Remove special chars and change to a single line
            fullQuery = rgx.Replace(fullQuery, " ");
            rgx = new Regex(@"[^0-9a-z ]+", RegexOptions.ECMAScript);
            fullQuery = rgx.Replace(fullQuery, "");

            List<string> words = fullQuery.Split(' ').ToList<string>();
            List<string> stopWords = GetStopWords();
            words = words.Where(e => !stopWords.Any(g => g == e)).ToList<string>(); //Turn string of search terms into a list, remove stopwords
            List<List<int>> result = new List<List<int>>();
            List<int> held;
            ConcurrentDictionary<int, List<int>> temp;
            bool nonePresent = true;
            words.Distinct();

            for (int i = 0; i < words.Count; i++)
            {
                lock (lock1) if (isStemming ?? false) words[i] = stemmer.StemWord(words[i]); //If stemming is on; stem the search term
                if (invertedIndex.ContainsKey(words[i]))
                {
                    invertedIndex.TryGetValue(words[i], out temp); 
                    held = temp.Keys.ToList();                      //Get a list of all file IDs containing the current search term 
                    result.Add(held);                               //Add that list to a collection of lists
                    nonePresent = false;
                }
            }

            if (nonePresent) listBox.Items.Add("No search terms had a match in the dataset.");
            else
            {

                if (searchType == 1)
                {
                    held = IntersectAll(result);
                } 
                else if (searchType == 2)
                {
                    held = CombineAll(result);
                }
                else
                {
                    held = SubtractIntersection(result);
                }

                foreach (int i in held)
                {
                    if(!listBox.Items.Contains(files[i])) listBox.Items.Add(files[i]); //Only list the file if it hasn't been listed already
                }
            }

            s.Stop();
            Console.WriteLine("Search Time: " + s.Elapsed);
        }

        public List<T> IntersectAll<T>(IEnumerable<IEnumerable<T>> lists)
        {
            HashSet<T> hashSet = new HashSet<T>(lists.First());
            foreach (var list in lists.Skip(1))
            {
                hashSet.IntersectWith(list); //Create a subset of file IDs that contain all search terms
            }
            return hashSet.ToList();
        }

        public List<T> CombineAll<T>(IEnumerable<IEnumerable<T>> lists)
        {
            List<T> tempList = new List<T>();
            foreach (var list in lists)
            {
                tempList.AddRange(list); //Add all File IDs to a single list
            }
            return tempList;
        }

        public List<T> SubtractIntersection<T>(IEnumerable<IEnumerable<T>> lists)
        {
            HashSet<T> hashSet = new HashSet<T>(lists.First());
            foreach (var list in lists.Skip(1))
            {
                hashSet.SymmetricExceptWith(list); //Collect all File IDs that contain ONE of the search terms but no others
            }
            return hashSet.ToList();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if(invertedIndex != null && searchParams != null)
            {
                listBox.Items.Clear();
                synonymsOn = synonymsBox.IsChecked;
                lock (lock1) isStemming = stemmingBox.IsChecked;
                if (and_Button.IsChecked ?? false) searchType = 1;
                else if (or_Button.IsChecked ?? false) searchType = 2;
                else searchType = 3;
                searchIndex(searchParams.Text);
            }
        }
    }
}
