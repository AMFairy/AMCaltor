﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Drawing;
using MSWord = Microsoft.Office.Interop.Word;

using System.IO;

using System.Reflection;


namespace CreateAnswerSheet
{
    class Program
    {
        static void Main(string[] args)
        {
            string tempdir = Environment.GetEnvironmentVariable("TEMP");

            object path;                               //文件路径变量

            string strContent;                         //文本内容变量

            MSWord.Application wordApp;                    //Word应用程序变量

            MSWord.Document wordDoc;                   //Word文档变量

            string dir = "";
            if(args.Length >= 2)
            {
                path = tempdir + "\\2645\\AMCalTor\\ans\\" + args[1] + "\\Ans.doc";
                dir = args[1];
            }
            else
            {
                path = tempdir + "\\2645\\AMCalTor\\ans\\Ans.doc";                 //路径
            }


            wordApp = new MSWord.ApplicationClass(); //初始化

            //如果已存在，则删除

            if (File.Exists((string)path))
            {

                File.Delete((string)path);

            }

            //由于使用的是COM库，因此有许多变量需要用Missing.Value代替

            Object Nothing = Missing.Value;

            wordDoc = wordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);

            //图片文件的路径

            string filename = "NULL";//@AppDomain.CurrentDomain.BaseDirectory + "1_00";

            //要向Word文档中插入图片的位置

            object count = 14;
            object WdParagraph = Microsoft.Office.Interop.Word.WdUnits.wdParagraph;//换一行;

            Object range;

            //定义该插入的图片是否为外部链接

            Object linkToFile = false;               //默认

            //定义要插入的图片是否随Word文档一起保存

            Object saveWithDocument = true;               //默认


            //2
            //const int chapter = 4;
            const int zNoCount = 5;
            const int tNoCountL = 1;
            int tNoCountR = 25;
            if (args.Length != 0)
                tNoCountR = Convert.ToInt32(args[0]);
            else
            {
                Console.Write("键入题量：");
                tNoCountR = Convert.ToInt32(Console.ReadLine());
            }

            bool flag_nexist = false;
            bool flag_nfirstParagraph = false;

            for (int tNoNum = tNoCountL; tNoNum <= tNoCountR; ++tNoNum)
            {
                int[] array = new int[6] { 0,1,2,3,4,5 };
                for (int i = 0; i <= 5; ++i) // 在0~5之间变换
                {
                    int zNoNum = array[i];
                    flag_nexist = false;
                    if (false == File.Exists(tempdir + "\\2645\\AMCalTor\\ans\\" + dir + "\\" + tNoNum.ToString() + "_" + zNoNum.ToString() + ".jpg"))
                        flag_nexist = true;

                    Console.WriteLine(tempdir + "\\2645\\AMCalTor\\ans\\" + dir + "\\" + tNoNum.ToString() + "_" + zNoNum.ToString() + ".jpg");
                  
                    //要向Word文档中插入图片的位置
                    filename = tempdir + "\\2645\\AMCalTor\\ans\\" + dir+ "\\" + tNoNum.ToString() + "_" + zNoNum.ToString() + ".jpg";


                    wordApp.Selection.MoveDown(ref WdParagraph, ref count, ref Nothing);//移动焦点
                    if (flag_nfirstParagraph && (zNoNum == 5 || zNoNum == 0))
                    {
                        wordApp.Selection.TypeParagraph();//插入段落
                    }
                    if (!flag_nfirstParagraph)
                    {
                        flag_nfirstParagraph = true;
                        wordApp.Selection.TypeText("您现在使用的是未付费版本，请自行寻找题号。");
                        wordApp.Selection.MoveDown(ref WdParagraph, ref count, ref Nothing);//移动焦点
                        wordApp.Selection.TypeParagraph();//插入段落
                    }


                    if (zNoNum == 5)
                        wordApp.Selection.TypeText("该题解析");
                    else if (zNoNum == 0)
                        wordApp.Selection.TypeText("某一题");
                    if (flag_nexist)
                    {
                        wordApp.Selection.TypeText("图片载入失败，自己做吧~");
                        continue;
                    }

                    Image pic = Image.FromFile(filename);//strFilePath是该图片的绝对路径
                    int intWidth = pic.Width;//长度像素值
                    int intHeight = pic.Height;//高度像素值 

                    wordApp.Selection.MoveDown(ref WdParagraph, ref count, ref Nothing);//移动焦点
                    wordApp.Selection.TypeParagraph();//插入段落
                 
                    range = wordDoc.Paragraphs.Last.Range;

                    //wordApp.Selection.TypeParagraph();//插入段落
                    //wordApp.Selection.MoveDown(ref WdParagraph, ref count, ref Nothing);//移动焦点
                    //使用InlineShapes.AddPicture方法插入图片

                    wordDoc.InlineShapes.AddPicture(filename, ref linkToFile, ref saveWithDocument, ref range);
                    
                    float useableWidth = wordDoc.ActiveWindow.Selection.PageSetup.PageWidth
                        - wordDoc.ActiveWindow.Selection.PageSetup.LeftMargin
                        - wordDoc.ActiveWindow.Selection.PageSetup.RightMargin;

                    Console.WriteLine("{0} {1} {2} {3}"
                        , intWidth, wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width
                        , intHeight, wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Height);
                    
                    wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Height = intHeight *3/4;
                    wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width = intWidth *3/4;

                    if (wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width > useableWidth)
                    {
                        wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width = useableWidth;
                        wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Height
                            = useableWidth / intWidth * intHeight;
                    }
                    //if (zNoNum == 1)
                    //{
                    //    float rate = 3.5f;
                    //    while (wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Height * rate > useableWidth / 5
                    //        || wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width * rate > useableWidth)
                    //        rate /= 2;
                    //    if (rate > 1)
                    //    {
                    //        wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width *= rate;
                    //        wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Height *= rate;
                    //    }

                    //}
                    //else if (zNoNum == 5)
                    //{
                    //    float rate = 400 / wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width;
                    //    wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width *= rate;
                    //    wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Height *= rate;
                    //}

                    Console.WriteLine("{0} {1} {2} {3}"
                        , intWidth, wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Width
                        , intHeight, wordDoc.Application.ActiveDocument.InlineShapes[wordDoc.Application.ActiveDocument.InlineShapes.Count].Height);

                }
            }
            //WdSaveFormat为Word 97文档的保存格式

            object format = MSWord.WdSaveFormat.wdFormatDocument97;

            //将wordDoc文档对象的内容保存为DOCX文档

            wordDoc.SaveAs(ref path, ref format, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);

            //关闭wordDoc文档对象

            wordDoc.Close(ref Nothing, ref Nothing, ref Nothing);

            //关闭wordApp组件对象

            wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);

            Console.WriteLine(path + " 创建完毕！");
            //Console.ReadKey(true);

        }
    }
}
