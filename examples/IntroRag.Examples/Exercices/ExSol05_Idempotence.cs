using System.Security.Cryptography;
using System.Text;

namespace IntroRag.Examples;

// Exercice 5 (solution) — ID déterministe : même texte => même ID (pas de doublon).
public static class ExSol05
{
    public static Task Run()
    {
        static string Md5(string s) => Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(s)));
        Console.WriteLine("Md5 identique ? " + (Md5("procedure reseau") == Md5("procedure reseau")));  // True
        return Task.CompletedTask;
    }
}
