using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TagsCloudApp.WordFiltering
{
    public class Filter: IWordFilter
    {
        private readonly string[] unusedPartsOfSpeech = { "PR", "ADV", "CONJ", "PART", "SPRO" };

        public Result<IEnumerable<string>> FilterWords(IEnumerable<string> words)
        {

            if (!File.Exists(@"..\..\mystem.exe"))
            {
                return Result.Fail<IEnumerable<string>>($"Cannot find file mystem.exe");
            }

            File.WriteAllLines("temp.txt", words);

            var result = "";
            var n = "";

            using (var myStemProcess = new Process())
            {
                var zed =
                myStemProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                myStemProcess.StartInfo.FileName = @"..\..\mystem.exe";
                myStemProcess.StartInfo.Arguments = "-ni temp.txt";
                myStemProcess.StartInfo.CreateNoWindow = true;
                myStemProcess.StartInfo.UseShellExecute = false;
                myStemProcess.StartInfo.RedirectStandardInput = true;
                myStemProcess.StartInfo.RedirectStandardOutput = true;
                myStemProcess.Start();
                result += myStemProcess.StandardOutput.ReadToEnd();
            }
            File.Delete("temp.txt");
            //return result;
            
            words = words.Select(w => w.ToLower());
            var myStemOutput = result;
            var filteredWords = myStemOutput
                .Split('\n')
                .Where(s => s != "" && !unusedPartsOfSpeech.Any(p => s.Contains($"={p}")))
                .Select(s => s.Split('{')[0]);
            return Result.Ok(filteredWords);
        }

        private string GetMyStemOutput(IEnumerable<string> words)
        {
            File.WriteAllLines("temp.txt", words);

            var result = "";

            using (var myStemProcess = new Process())
            {
                var zed = 
                myStemProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8; 
                myStemProcess.StartInfo.FileName = @"..\..\mystem.exe";
                myStemProcess.StartInfo.Arguments = "-ni temp.txt";
                myStemProcess.StartInfo.CreateNoWindow = true;
                myStemProcess.StartInfo.UseShellExecute = false;
                myStemProcess.StartInfo.RedirectStandardInput = true;
                myStemProcess.StartInfo.RedirectStandardOutput = true;
                myStemProcess.Start();
                result += myStemProcess.StandardOutput.ReadToEnd();
            }
            File.Delete("temp.txt");
            return result;
        }
    }
}
