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
    private int _viewportWidth;



    //Variaveis do foguete

    private Vector2 _starshipPosition;
    private float _starshipSpeed = 80f;

    //Variaveis de velocidade
    private float _speed = 200f;

    //Variaveis das estrelas e contagens
    private Vector2 _starPosition;
    private int _collectedStars = 0;
    private Random _random = new Random();
    private Boolean _starCollected = false;

    //Variaveis do meteoro
    private Vector2 _meteorPosition;
    private float _timeMeteor = 0; 

    //Variaveis de estado de jogo e menu
    public enum GameState{
        Menu,
        Active,
        Credits,
        EndGame
    }
    private double menuTimer = 0;
    private GameState _currentGameState = GameState.Menu;
    private int _menuSelected = 0;
    private string[] _menuItems = {"Comecar jogo","Creditos", "Sair"};
    private SpriteFont _font;

    //Textura
    private Texture2D _starshipTexture;
    private Texture2D _meteorTexture;
    private Texture2D _starTexture;    
    private Texture2D _starCollectedTexture;


    //Som
    private SoundEffect _soundCollect;
    private SoundEffect _soundCrash;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        _viewportWidth = GraphicsDevice.Viewport.Width;
        _starshipPosition = new Vector2(50, GraphicsDevice.Viewport.Height / 2);
        _starPosition = new Vector2(_viewportWidth, 0);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _starTexture = Content.Load<Texture2D>("estrelaPadrão");
        _starCollectedTexture = Content.Load<Texture2D>("estrelaColetada");

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
        
        if(_currentGameState == GameState.Menu){//se estiver no menu
            menuTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if(menuTimer > .5){
                if (_keyboard.IsKeyDown(Keys.W) && _menuSelected > 0 ){// menu pra cima
                    _menuSelected--;
                    menuTimer = 0;
                }
                if (_keyboard.IsKeyDown(Keys.S) && _menuSelected < _menuItems.Length - 1){// menu pra baixo
                    _menuSelected++;
                    menuTimer = 0;
                }                
            }
            if (_keyboard.IsKeyDown(Keys.Enter)){//selecionar
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
            
        }else if (_currentGameState == GameState.Active){// se estiver no jogo
            //verificações de estrela
            _starPosition.X -= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;           
            
            Rectangle starshipRectangle = new Rectangle((int)_starshipPosition.X, (int)_starshipPosition.Y, _starshipTexture.Width, _starshipTexture.Height);
            Rectangle starRectangle = new Rectangle((int)_starPosition.X, (int)_starPosition.Y, _starTexture.Width, _starTexture.Height);
            Rectangle meteorRectangle = new Rectangle((int)_meteorPosition.X, (int)_meteorPosition.Y, _meteorTexture.Width, _meteorTexture.Height);


            if(_starPosition.X < - _starTexture.Width){
                _starPosition = new Vector2(_viewportWidth, _random.Next(0, GraphicsDevice.Viewport.Height - _starTexture.Height));
                _starCollected = false;
            }

            if (starshipRectangle.Intersects(starRectangle)) {
                if(!_starCollected){
                    _soundCollect.Play();
                    _collectedStars++;
                    if(_collectedStars == 40){
                        _currentGameState = GameState.EndGame;
                    }
                }
                
                _starCollected = true;
            }

            if(_collectedStars == 8){
                _speed = 250f;
                _starshipSpeed = 100f;
            } else if (_collectedStars == 18){
                _speed = 350f;
                _starshipSpeed = 120f;
            } 
            

            //Verificação do meteoro
            if(_timeMeteor < 3){
                _timeMeteor += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _meteorPosition.Y = _viewportWidth; // não consegui colocar ele no initialize, não sei o porquê
            }

            if(_timeMeteor > 3){
                _meteorPosition.X -= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if(_meteorPosition.X < - _meteorTexture.Width){
                _meteorPosition = new Vector2(_viewportWidth, _random.Next(0, GraphicsDevice.Viewport.Height - _meteorTexture.Height));
            }

            if(starshipRectangle.Intersects(meteorRectangle)){
                _soundCrash.Play();
                _currentGameState = GameState.EndGame;
            }

            //verificações de teclado
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
        }else if(_currentGameState == GameState.Credits){// creditos
            if(_keyboard.IsKeyDown(Keys.Space)){
                _currentGameState = GameState.Menu;
            }
        } else if(_currentGameState == GameState.EndGame){// Fim de jogo
            if(_keyboard.IsKeyDown(Keys.Space)){
                _currentGameState = GameState.Menu;
                _meteorPosition.X = _viewportWidth;
                _starPosition.X = _viewportWidth;
                _timeMeteor = 0;
                _collectedStars = 0;
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();


        if(_currentGameState == GameState.Menu){// tela de menu
            for(int i = 0; i < _menuItems.Length; i++){
                Color color = i == _menuSelected ? Color.Yellow : Color.White;
                
                _spriteBatch.DrawString(_font, _menuItems[i], new Vector2(100, 100 + i * 20), color);
            }
            _spriteBatch.DrawString(_font, "Aperte ENTER para selecionar", new Vector2(100, 300), Color.White);

        }else if(_currentGameState == GameState.Active){ //Jogo ativo
            _spriteBatch.Draw(_starshipTexture, _starshipPosition, Color.White);

            if(!_starCollected){
                _spriteBatch.Draw(_starTexture, _starPosition, Color.White);
            } else{
                _spriteBatch.Draw(_starCollectedTexture, _starPosition, Color.White);
            }

            _spriteBatch.Draw(_meteorTexture, _meteorPosition, Color.Wheat);

            _spriteBatch.DrawString(_font, _collectedStars.ToString(), new Vector2(_viewportWidth / 2, 20), Color.White);
        } else if(_currentGameState == GameState.Credits){//Créditos 
            _spriteBatch.DrawString(_font, "Creditos: Thiago Pininga Tavares", new Vector2(100, 100), Color.White);
            _spriteBatch.DrawString(_font, "Aperte ESPACO para voltar para o menu", new Vector2(100, 200), Color.White);

        } else if(_currentGameState == GameState.EndGame){// Fim de jogo
            if(_collectedStars == 40){
                _spriteBatch.DrawString(_font, "Parabens, voce venceu!", new Vector2(100, 100), Color.White);
            } else{
                _spriteBatch.DrawString(_font, "Voce perdeu", new Vector2(100, 100), Color.White);
            }
            _spriteBatch.DrawString(_font, "Aperte ESPACO para voltar ao menu", new Vector2(100, 200), Color.White);

        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
