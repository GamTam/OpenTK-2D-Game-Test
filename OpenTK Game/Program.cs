// See https://aka.ms/new-console-template for more information

using Open_TK_Tut_1;
using OpenTK.Windowing.Common.Input;

using (Game myGame = new Game(1024, 768, "Professor Layton and the Hunt for Descole"))
{
    myGame.Run();
}