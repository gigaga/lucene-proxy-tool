using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;

namespace lucene_proxy_tool
{
    class Program
    {

        internal class Result
        {
            public List<Dictionary<string, string>> docs { get; set; }

            public int nbHits { get; set; }
        }


        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)

        {

            string dllName = args.Name.Contains(',') ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");

            dllName = dllName.Replace(".", "_");

            if (dllName.EndsWith("_resources")) return null;

            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Program).Namespace + ".Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());

            byte[] bytes = (byte[])rm.GetObject(dllName);

            return System.Reflection.Assembly.Load(bytes);

        }

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        static void Main(string[] args)
        {
            // redirect console output to parent process;
            // must be before any calls to Console.WriteLine()
            AttachConsole(ATTACH_PARENT_PROCESS);

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            String indexFolder=null;
            String field = null;
            String token = null;
            String output = null;
            int maxResult = 0;
            try {
                indexFolder = args[0];
                // Execution from lptf file
                if (indexFolder.EndsWith(".lptf"))
                {
                    string text = System.IO.File.ReadAllText(@indexFolder);
                    args = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    indexFolder = args[0];
                }
                field = args[1];
                token = args[2];
                maxResult = Convert.ToInt16(args[3]);
                output = args[4];
            } catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Syntax error:");
                Console.WriteLine("\targuments are: <indexFolder> <field> <token> <maxResult> <fileOutput|stdout>");
                System.Environment.Exit(1);
            }
            Result result = null;

            try
            {
                result = search(indexFolder, field, token, maxResult);
            } catch (Exception e)
            {
                Dictionary<string, string> dicError = new Dictionary<string, string>();
                dicError.Add("errorMessage", e.Message);
                String jsonError = new JavaScriptSerializer().Serialize(dicError);
                if (output == "stdout")
                {
                    Console.WriteLine(jsonError);
                }
                else
                {
                    System.IO.File.WriteAllLines(@output, new string[] { jsonError });
                }
                System.Environment.Exit(1);
            }
            String jsonResult = new JavaScriptSerializer().Serialize(result);
            if (output == "stdout") {
                Console.WriteLine(jsonResult);
            } else
            {
                System.IO.File.WriteAllLines(@output, new string[] { jsonResult });
            }

            System.Environment.Exit(0);
        }

        static Result search(String indexFolder, String field, String token, int maxResult)
        {
            IndexReader indexReader = IndexReader.Open(FSDirectory.Open(indexFolder), true);

            IndexSearcher indexSearcher = new IndexSearcher(indexReader);

            Analyzer analyser = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_CURRENT);
            QueryParser queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_CURRENT, field, analyser);

            Query query = queryParser.Parse(token);
            TopDocs results = indexSearcher.Search(query, maxResult);

            Result result = new Result();

            result.nbHits = results.TotalHits;
            result.docs = new List<Dictionary<string, string>>();

            foreach (var item in results.ScoreDocs)
            {
                Document doc = indexSearcher.Doc(item.Doc);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var docField in doc.GetFields())
                {
                    string fieldName = ((Field)docField).Name;
                    dic.Add(fieldName, doc.Get(fieldName));
                }
                result.docs.Add(dic);
            }

            return result;
        }
    }
}
