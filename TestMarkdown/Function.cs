using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace TestMarkdown {
    class Function {
        public static string[] FindFile(string path) {
            string[] fileNames;
            fileNames = Directory.GetFiles(path);
            return fileNames;
        }

        public static string[] OpenFile(string[] fileNames) {
            int linesCount = 0;

            for (int i = 0; i < fileNames.Length; i++) {
                linesCount += File.ReadAllLines(fileNames[i]).Length;
            }

            string[] lines = new string[linesCount];            

            int pos = 0;
            foreach (string filename in fileNames) {
                string[] temp = Array.Empty<string>();
                temp = File.ReadAllLines(filename);                
                temp.CopyTo(lines, pos);
                pos += temp.Length;
            }

            return lines;
        }

        public static List<string> FindTable(string[] lines) {
            List<string> tableWrong = new List<string>();
            int startIndex = 0;
            bool isTitled, hasHeader, isTable;
            isTitled = hasHeader = isTable = false;
            Regex table = new Regex(@"\|?\s*\:?\-\-\-\-*\:?\s\|?(\|?\s*\:?\-\-\-\-*\:?\s\|?)*");
            Regex tableTitle = new Regex(@"Таблица\s\d\.\s*..*", RegexOptions.Singleline);
            Regex tableRow = new Regex(@"(?(?=(?:.*\|){1,})((\|)|(\s*))\s*[^\s]+(\s\s*\|?|\s*\|?)|\|)(((\|)|(\s*))\s*[^\s]+(\s\s*\|?|\s*\|?))*", RegexOptions.Singleline);
            Regex whiteSpace = new Regex(@"^\s*$");

            for (int j = 0; j < lines.Length; j++) {
                MatchCollection whiteSpaceMatches = whiteSpace.Matches(lines[j]);
                MatchCollection tableTitleMatches = tableTitle.Matches(lines[j]);
                MatchCollection tableRowMatches = tableRow.Matches(lines[j]);
                MatchCollection tableMatches = table.Matches(lines[j]);
                if (tableTitleMatches.Count > 0) {
                    foreach (Match match in tableTitleMatches) {
                        tableWrong.Add(match.Value);
                    }
                    isTitled = true;
                }
                if (tableMatches.Count > 0) {
                    if (hasHeader == true)
                        isTable = true;
                }
                if (tableRowMatches.Count > 0) {
                    foreach (Match match in tableRowMatches) {
                        tableWrong.Add(match.Value);
                    }
                    if (isTable == false)
                        hasHeader = true;
                }
                if ((whiteSpaceMatches.Count > 0) || (j == lines.Length - 1)) {
                    if ((isTitled && hasHeader && isTable) || !isTable) {
                        tableWrong.RemoveRange(startIndex, tableWrong.Count - startIndex);
                        isTitled = hasHeader = isTable = false;
                    }
                    if (!isTitled && hasHeader && isTable) {
                        tableWrong.Add("\n");
                        startIndex = tableWrong.Count;
                        isTitled = hasHeader = isTable = false;
                    }
                }
            }
            return tableWrong;
        }



        public static List<string> FindImage(string[] lines) {
            List<string> imageWrong = new List<string>();
            int startIndex = 0;
            bool isTitled, isImage;
            isTitled = isImage = false;
            Regex image = new Regex(@"!\[\S*\]\(\S*\)");
            Regex imageTitle = new Regex(@"Изображение\s\d\.\s*..*", RegexOptions.Singleline);
            Regex whiteSpace = new Regex(@"^\s*$");

            for (int j = 0; j < lines.Length; j++) {
                MatchCollection whiteSpaceMatches = whiteSpace.Matches(lines[j]);
                MatchCollection imageMatches = image.Matches(lines[j]);
                MatchCollection imageTitleMatches = imageTitle.Matches(lines[j]);

                if (imageTitleMatches.Count > 0) {
                    foreach (Match match in imageTitleMatches) {
                        imageWrong.Add(match.Value);
                    }
                    isTitled = true;
                }
                if (imageMatches.Count > 0) {
                    foreach (Match match in imageMatches) {
                        imageWrong.Add(match.Value);
                    }
                    isImage = true;
                }
                if (((whiteSpaceMatches.Count > 0 && (isImage)) || (j == lines.Length - 1))) {
                    if ((isTitled && isImage) || !isImage) {
                        imageWrong.RemoveRange(startIndex, imageWrong.Count - startIndex);
                        isTitled = isImage = false;
                    }
                    if (!isTitled && isImage) {
                        imageWrong.Add("\n");
                        startIndex = imageWrong.Count;
                        isTitled = isImage = false;
                    }
                }
            }
            return imageWrong;
        }

        public static int CountTables(string[] lines) {
            int count = 0;
            Regex table = new Regex(@"\|?\s*\:?\-\-\-\-*\:?\s\|?(\|?\s*\:?\-\-\-\-*\:?\s\|?)*");

            for (int j = 0; j < lines.Length; j++) {
                MatchCollection imageMatches = table.Matches(lines[j]);
                if (imageMatches.Count > 0) {
                    count++;
                }
            }
            return count;
        }

        public static int CountImages(string[] lines) {
            int count = 0;
            Regex image = new Regex(@"!\[\S*\]\(\S*\)");

            for (int j = 0; j < lines.Length; j++) {
                MatchCollection tableMatches = image.Matches(lines[j]);
                if (tableMatches.Count > 0) {
                    count++;
                }
            }
            return count;
        }

        public static void Menu(string path) {
            int command;
            string[] lines = OpenFile(FindFile(path));
            string menu = " 1. Суммарное количество таблиц в файлах \n 2. Суммарное количество изображений в файлах\n 3. Таблицы без подписи\n 4. Изображения без подписи\n 5. Выход";
            Console.WriteLine(menu);
            do {
                Console.Write("Введите номер команды: ");
                command = Convert.ToInt32(Console.ReadLine());
                switch (command) {
                    case 1:
                        Console.WriteLine("Количество таблиц - " + CountTables(lines));
                        break;
                    case 2:
                        Console.WriteLine("Количество изображений - " + CountImages(lines));
                        break;
                    case 3:
                        Console.WriteLine("У данных таблиц отсутствуют подписи или они записаны неккоректно.");
                        for (int i = 0; i < FindTable(lines).Count; i++)
                            Console.WriteLine(FindTable(lines)[i]);
                        break;
                    case 4:
                        Console.WriteLine("У данных изображений отсутствуют подписи или они записаны неккоректно.");
                        for (int i = 0; i < FindImage(lines).Count; i++)
                            Console.WriteLine(FindImage(lines)[i]);
                        break;
                    case 5:
                        break;
                    default:
                        Console.WriteLine("Неверная команда");
                        break;
                }
            } while (command != 5);

        }
    }
}


