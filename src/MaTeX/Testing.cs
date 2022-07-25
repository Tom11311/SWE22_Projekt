using Xunit;
using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;

public class Testings
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MathToLatex_Vector_ReturnLatex(bool prettyPrinting)
    {
        MaTeX.Config.PrettyPrinting = prettyPrinting;
        Vector A = DenseVector.OfArray(new double[] {1,2,3});
        string Latex = (prettyPrinting   
            ? @"\begin{pmatrix}" + "\n" + @"1 \\" + "\n" + @"2 \\" + "\n" + "3 \n" + @"\end{pmatrix}"
            : @"\begin{pmatrix}1\\2\\3\end{pmatrix}"
        );
        Assert.Equal(Latex, MaTeX.Conv.MathToLatex(A));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MathToLatex_Matrix_ReturnLatex(bool prettyPrinting)
    {
        MaTeX.Config.PrettyPrinting = prettyPrinting;
        Matrix B = DenseMatrix.OfArray(new double[,] {
            {1,1,1},
            {1,2,3},
            {4,3,2}
        });
        string Latex = (prettyPrinting
            ? @"\begin{bmatrix}" + "\n" + @"1 & 1 & 1 \\" + "\n" + @"1 & 2 & 3 \\" + "\n" + "4 & 3 & 2 " + "\n" + @"\end{bmatrix}"
            : @"\begin{bmatrix}1&1&1\\1&2&3\\4&3&2\end{bmatrix}"
        ); 
        Assert.Equal(Latex, MaTeX.Conv.MathToLatex(B));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MathToLatex_Gleichung_ReturnLatex(bool prettyPrinting)
    {
        MaTeX.Config.PrettyPrinting = prettyPrinting;
        string Gleichung = "f=0=3*3+sqrt(sqrt(a))";
        string Latex = (prettyPrinting   
            ? @"f = 0 = 9 + \sqrt{\sqrt{a}}"
            : @"f=0=9+\sqrt{\sqrt{a}}"
        );
        Assert.Equal(Latex, MaTeX.Conv.MathToLatex(Gleichung));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MathToLatex_Term_ReturnLatex(bool prettyPrinting)
    {
        MaTeX.Config.PrettyPrinting = prettyPrinting;
        string Term = @"3*3+sqrt(sqrt(a))";
        string Latex = (prettyPrinting
            ? @"9 + \sqrt{\sqrt{a}}"
            : @"9+\sqrt{\sqrt{a}}"
        );
        Assert.Equal(Latex, MaTeX.Conv.MathToLatex(Term));
    }

    private bool FileCompare(string file1, string file2)
    {
        int file1Byte;
        int file2Byte;
        FileStream fileStream1;
        FileStream fileStream2;

        if (file1 == file2) return true;

        fileStream1 = new FileStream(file1, FileMode.Open);
        fileStream2 = new FileStream(file2, FileMode.Open);

        if (fileStream1.Length != fileStream2.Length)
        {
            fileStream1.Close();
            fileStream2.Close();
            return false;
        }

        do
        {
            file1Byte = fileStream1.ReadByte();
            file2Byte = fileStream2.ReadByte();
        }
        while ((file1Byte == file2Byte) && (file1Byte != -1));

        fileStream1.Close();
        fileStream2.Close();

        return ((file1Byte - file2Byte) == 0);
    }

    
    [Theory]
    [InlineData("R_AsText.tex")]
    [InlineData("S_AsText.txt")]
    [InlineData("V_AsText.md")]
    [InlineData("M_AsText.ltx")]
    [InlineData("debug.tex")]
    public void AsText_Latex_File(string textFormat)
    {
        MaTeX.Config.SaveLocation = Directory.GetParent(@"bin").FullName;
        File.Create(textFormat);
        MaTeX.Config.PrettyPrinting = true;
        
        string fileName = textFormat;
        string comparePath = Path.GetFullPath("TestFiles/" + fileName);

        switch (textFormat)
        {
            case "R_AsText.tex":
                Vector y = DenseVector.OfArray(new double[] {4,7,1});
                Matrix I = DenseMatrix.OfArray(new double[,] {
                    {1,0,0},
                    {0,1,0},
                    {0,0,1}
                });
                Matrix A = DenseMatrix.OfArray(new double[,] {
                    {0,5,4},
                    {4,8,0},
                    {2,9,9}
                });

                string y_latex = MaTeX.Conv.MathToLatex(y);
                string I_latex = MaTeX.Conv.MathToLatex(I);
                string A_latex = MaTeX.Conv.MathToLatex(A);
                string R_latex = MaTeX.Conv.MathToLatex("R");

                string latex = A_latex
                    + "-" + MaTeX.Wrapper.PrettyPrint("\n")
                    + y_latex + @"\cdot" + MaTeX.Wrapper.PrettyPrint("\n")
                    + I_latex + "=" + R_latex;
                Console.WriteLine(latex);

                MaTeX.Export.AsText(
                    latex,
                    fileName,
                    MaTeX.WriteModes.OVERRIDE,
                    MaTeX.TextFormats.TEX_DOCUMENT,
                    new MaTeX.BracketModes[] {MaTeX.BracketModes.BEGIN, MaTeX.BracketModes.END}
                );
                break;
            
            case "S_AsText.txt":
                string S = "0 = 3*2-sqrt(x)";
                string S_latex = MaTeX.Conv.MathToLatex(S);
                MaTeX.Export.AsText(S_latex, "S_AsText", MaTeX.WriteModes.OVERRIDE, MaTeX.TextFormats.TXT);
                break;

            case "V_AsText.md":
                Vector V = DenseVector.OfArray(new double[] {4,7,1});
                string V_latex = MaTeX.Conv.MathToLatex(V);
                MaTeX.Export.AsText(V_latex, "V_AsText", MaTeX.WriteModes.OVERRIDE, MaTeX.TextFormats.MD);
                break;
            
            case "M_AsText.ltx":
                Matrix M = DenseMatrix.OfArray(new double[,] {
                    {1,5,0},
                    {0,3,0},
                    {4,0,1}
                });
                string M_latex = MaTeX.Conv.MathToLatex(M);
                MaTeX.Export.AsText(M_latex, "M_AsText.ltx", MaTeX.WriteModes.OVERRIDE, MaTeX.TextFormats.TEX_DOCUMENT);
                break;

            case "debug.tex":
                MaTeX.Config.BracketMode = new MaTeX.BracketModes[] {};

                Console.WriteLine(MaTeX.Export.AsText(
                    MaTeX.Wrapper.PrettyPrint("\n")
                        + @"\text{OVERRIDE TEX_DOCUMENT}"
                        + MaTeX.Wrapper.PrettyPrint("\n"),
                    fileName,
                    MaTeX.WriteModes.OVERRIDE,
                    MaTeX.TextFormats.TEX_DOCUMENT
                ));
                
                MaTeX.Export.AsText(
                    MaTeX.Wrapper.PrettyPrint("\n")
                        + @"\text{INSERT_AFTER_DOCUMENT_START TEX}"
                        + MaTeX.Wrapper.PrettyPrint("\n"),
                    fileName,
                    MaTeX.WriteModes.INSERT_AFTER_DOCUMENT_START,
                    MaTeX.TextFormats.TEX,
                    MaTeX.BracketModes.BEGIN
                );

                MaTeX.Export.AsText(
                    MaTeX.Wrapper.PrettyPrint("\n")
                        + @"\text{INSERT_BEFORE_DOCUMENT_END TEX}"
                        + MaTeX.Wrapper.PrettyPrint("\n"),
                    fileName,
                    MaTeX.WriteModes.INSERT_BEFORE_DOCUMENT_END,
                    MaTeX.TextFormats.TEX,
                    MaTeX.BracketModes.END                
                );
                break;
        }

        string path = Path.Combine(Directory.GetParent(@"bin").FullName, textFormat);
        Assert.True(FileCompare(path, comparePath));
        File.Delete(path);
    }
}