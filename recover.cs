using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class Program {
    static void Main() {
        string[] lines = File.ReadAllLines("dblog.txt");
        HashSet<string> foundTexts = new HashSet<string>();
        foreach (string line in lines) {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts) {
                if (p.StartsWith("0x") && p.Length > 4) {
                    try {
                        string hex = p.Substring(2);
                        if (hex.Length % 2 != 0) continue;
                        byte[] bytes = new byte[hex.Length / 2];
                        for (int i = 0; i < bytes.Length; i++) {
                            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                        }
                        
                        // Decode as UTF-16LE
                        string text = Encoding.Unicode.GetString(bytes);
                        
                        // Extract words/HTML
                        var matches = Regex.Matches(text, @"[\w\s<>/=\"":;&\-.,À-?]+");
                        foreach (Match m in matches) {
                            if (m.Value.Length > 50 && m.Value.Contains(" ")) {
                                foundTexts.Add(m.Value.Trim());
                            }
                        }
                    } catch {}
                }
            }
        }
        foreach(var t in foundTexts) {
            Console.WriteLine("-------------------");
            Console.WriteLine(t);
        }
    }
}
