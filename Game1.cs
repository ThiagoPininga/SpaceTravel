using System;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    //Variaveis do foguete
    private Vector2 _starshipPosition;
    private float _starshipSpeed = 80f;

    //Variaveis de velocidade
    private float _speed = 70f;

    //Variaveis de estado de jogo e menu
    public enum GameState{
        Menu,
        Active,
        Credits,
        EndGame
    }

    private GameState _currentGameState = GameState.Menu;
    private int _menuSelected = 0;
    private string[] _menuItems = {"Começar jogo", "Escolher Dificuldade" ,"Creditos", "Sair"};
    private SpriteFont _font;

    //Textura
    private Texture2D _starshipTexture;
    private Texture2D _meteorTexture;
    private Texture2D _starTexture;

    //Som
    private SoundEffect _soundCollect;
    private SoundEffect _soundCrash;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _starshipPosition = new Vector2(50, GraphicsDevice.Viewport.Height / 2);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _starTexture = Content.Load<Texture2D>("estrela");
        _starshipTexture = Content.Load<Texture2D>("foguete");
        _meteorTexture = Content.Load<Texture2D>("meteoro");

        _soundCollect = Content.Load<SoundEffect>("Coletar");
        _soundCrash = Content.Load<SoundEffect>("dano");

        _font = Content.Load<SpriteFont>("GameFont");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState _keyboard = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyboard.IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        if(_currentGameState == GameState.Menu){
            if (_keyboard.IsKeyDown(Keys.W) && _menuSelected > 0){
                _menuSelected--;
            }
            if (_keyboard.IsKeyDown(Keys.S) && _menuSelected < _menuItems.Length - 1){
                _menuSelected++;
            }
            if (_keyboard.IsKeyDown(Keys.Enter)){
                switch (_menuSelected){
                    case 0: 
                        _currentGameState = GameState.Active;
                        break;
                    case 1: 
                        _currentGameState = GameState.Credits;
                        break;
                    case 2: 
                        Exit();
                        break;
                }
            }
        }else if (_currentGameState == GameState.Active){
            if(_keyboard.IsKeyDown(Keys.W)) {

                _starshipPosition.Y -= _starshipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(_starshipPosition.Y < 0){
                    _starshipPosition.Y = 0;
                }
            }
            if(_keyboard.IsKeyDown(Keys.S)) {
                _starshipPosition.Y += _starshipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(_starshipPosition.Y + _starshipTexture.Height > GraphicsDevice.Viewport.Height){
                    
                    _starshipPosition.Y = GraphicsDevice.Viewport.Height - _starshipTexture.Height;
                }
            }
        }else if(_currentGameState == GameState.Credits){
            if(_keyboard.IsKeyDown(Keys.Enter)){
                _currentGameState = GameState.Menu;
            }
        } else if(_currentGameState == GameState.EndGame){

        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();


        if(_currentGameState == GameState.Menu){
            for(int i = 0; i < _menuItems.Length; i++){
                Color color = i == _menuSelected ? Color.Yellow : Color.White;
                
                _spriteBatch.DrawString(_font, _menuItems[i], new Vector2(100, 100 + i * 20), color);

            }
        }else if(_currentGameState == GameState.Active){
            _spriteBatch.Draw(_starshipTexture, _starshipPosition, Color.White);
        } else if(_currentGameState == GameState.Credits){

        } else if(_currentGameState == GameState.EndGame){
            Exit();
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
