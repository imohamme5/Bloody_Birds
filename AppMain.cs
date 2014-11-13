using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;

//enum to control game state
enum gS
{
	//Start screen
	START = 0,
	
	//Screen where main game takes place
	GAME = 1,
	
	//Post game score screen
	SCORE = 2,
	
	//High Score table
	HSCORE = 3,
	
	//Options Screen
	OPTION = 4
}

namespace Bloody_Birds
{
	public class AppMain
	{
		private static Sce.PlayStation.HighLevel.GameEngine2D.Scene 	gameScene;
		private static Sce.PlayStation.HighLevel.UI.Scene 				uiScene;
		private static Sce.PlayStation.HighLevel.UI.Label				scoreLabel;
		private static Sce.PlayStation.HighLevel.UI.Label				titleLabel;
		private static Sce.PlayStation.HighLevel.UI.Label[]				scoreBoardLabels;
		
		private static bool 				quitGame;
		private static int 					score;
		private static int					timer;
		private static string				scoreString;
		private static gS					gameState;
		private static int[] 				scoreBoard;
		private static int 					scoreSlotCount;
		
		public static void Main (string[] args)
		{
			quitGame = false;
			Initialize ();
			
			//Game Loop
			while (!quitGame) 
			{
				Update ();
				
				Director.Instance.Update();
				Director.Instance.Render();
				UISystem.Render();
				
				Director.Instance.GL.Context.SwapBuffers();
				Director.Instance.PostSwap();
			}
			//Game ended, time to clean up
		}
		
		public static void Initialize ()
		{
			
			gameState = gS.START;
			
			//initialise score values
			score = 0;
			timer = 0;
			scoreSlotCount = 6;
			scoreBoard = new int[scoreSlotCount];
			scoreString = score.ToString(scoreString);
			
			
			Director.Initialize ();
			UISystem.Initialize(Director.Instance.GL.Context);
			
			//Set game scene
			gameScene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			gameScene.Camera.SetViewFromViewport();
			
			//Set the ui scene.
			uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			
			//Setup Panel
			Panel panel  = new Panel();
			panel.Width  = Director.Instance.GL.Context.GetViewport().Width;
			panel.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			//Setup Labels
			scoreLabel = makeLabel(scoreLabel, panel, -300, 2);
			titleLabel = makeLabel(titleLabel, panel, -100, 50);
			scoreBoardLabels = new Sce.PlayStation.HighLevel.UI.Label[scoreSlotCount];
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				scoreBoardLabels[i] = makeLabel(scoreBoardLabels[i], panel, 50, i*100);
			}
			UISystem.SetScene(uiScene);
			
			//Run the scene.
			Director.Instance.RunWithScene(gameScene, true);
		}

		public static void Update ()
		{
			
			/*
			 For now, the game is meant to advance from start > game > score > hscore > start
			 then back through
			 
			 Once this works we have proof that this system for changing game screens/states works
			 and I can then move on to menus and the scoring system in more detail
			 
			 
			 13/11 Update
			 The system detailed above works and a high score table has been implemented along with labels for each screen
			 with its title on, these are not neccesarily final names/screens
			 */
			
			// check to see if screen has been touched
			var touch = Touch.GetData (0);
			
			//Set scoreleval to the current value of Score
			scoreLabel.Text = score.ToString ();
			
			//Timer controls how often a touch can be registered,
			//a touch is recognised only when timer <= 0
			if(timer > 0)
			timer--;
			
			//gs.Start = Start screen
			if(gameState == gS.START)
			{
				titleLabel.Text = "Start Screen";
				if(touch.Count > 0 && timer <= 0)
				{
					gameState = gS.GAME;
					timer = 50;
				}
				
			}
			
			//gs.GAME = main game screen
			if(gameState == gS.GAME)
			{
				titleLabel.Text = "Main Game Screen";
				score++;
				if(touch.Count > 0 && timer <= 0)
				{
					gameState = gS.SCORE;
					timer = 50;
					scoreCalc();
					
				}
			}
			
			//gs.SCORE = post defeat/victory score screen
			if(gameState == gS.SCORE)
			{
				titleLabel.Text = "Score Screen";
				if(touch.Count > 0 && timer <= 0)
				{
					gameState = gS.HSCORE;
					timer = 50;
					for(int i = 0; i < scoreSlotCount - 1; i++)
					{
						scoreBoardLabels[i].Visible = true;
						scoreBoardLabels[i].Text = scoreBoard[i].ToString ();
					}
				}
				
			}
			
			//gs.HSCORE = end of game score screen, loops back to start screen
			if(gameState == gS.HSCORE)
			{
				titleLabel.Text = "High Score Screen";
				if(touch.Count > 0 && timer <= 0)
				{
					gameState = gS.START;
					timer = 50;
					score = 0;
					for(int i = 0; i < scoreSlotCount - 1; i++)
					{
					scoreBoardLabels[i].Visible = false;
					}
				}
				
			}
		}
		
		public static void scoreCalc()
		{
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				int temp;
				int temp2;
				if(scoreBoard[i] < score)
				{
					temp = scoreBoard[i];
					scoreBoard[i] = score;
					while(i < scoreSlotCount - 1)
					{
						i++;
						temp2 = scoreBoard[i];
						scoreBoard[i] = temp;
						temp = temp2;
					}
				}
			}
		}
		
		public static Sce.PlayStation.HighLevel.UI.Label makeLabel(Sce.PlayStation.HighLevel.UI.Label l, Panel p, int w, int h)
		{
			l = new Sce.PlayStation.HighLevel.UI.Label();
			l.HorizontalAlignment = HorizontalAlignment.Center;
			l.VerticalAlignment = VerticalAlignment.Top;
			l.SetPosition(
				Director.Instance.GL.Context.GetViewport().Width/2 + w,
				Director.Instance.GL.Context.GetViewport().Height/8 + h);
			l.Text = "";
			p.AddChildLast(l);
			uiScene.RootWidget.AddChildLast(p);
			return l;
		}
	}
}
