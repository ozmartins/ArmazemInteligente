using System;
using System.Collections.Generic;
using System.Linq;
using ArmazemInteligente.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ArmazemInteligente;

public class WarehouseGame : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private EventBus _bus;
    private Blackboard _bb;
    private List<Agent> _agents;
    private Texture2D _dockTexture;
    private Texture2D _emptyForkliftTexture;
    private Texture2D _loadedForkliftTexture;
    private Texture2D _palletTexture;
    private Texture2D _truckTexture;
    private Texture2D _greenTrafficLightTexture;
    private Texture2D _yellowTrafficLightTexture;
    private Texture2D _redTrafficLightTexture;

    public WarehouseGame() 
    {
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth  = display.Width;
        _graphics.PreferredBackBufferHeight = display.Height;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
    }

    protected override void Initialize() 
    {
        _bus = new EventBus();
        _bb = new Blackboard();
        
        SeedWorld(_bb.Warehouse);

        var dock = _bb.Warehouse.Docks.First();
        
        _agents = 
        [
            new DockAgent("Dock1", _bus, _bb, dock),
            new ErpAgent(_bus, _bb),
            new ForkliftAgent("Forklift1", _bus, _bb, new Vector2(64, 64)),
            new VisionAgent(_bus, _bb)
        ];
        
        base.Initialize();
    }

    private void SeedWorld(Warehouse wh) 
    {
        DisplayMode display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        var screenWidth  = display.Width;
        for (int i = 1; i <= 5; i++)
        {
            wh.Docks.Add(new Dock(1, new Vector2((screenWidth / 5f *i) -_dockTexture.Width / 5f, 0850), _dockTexture));
        }

        wh.Pallets.Add(new Pallet("P001", "RACAO-DOG-10KG", new Vector2(120, 300)));
        wh.Pallets.Add(new Pallet("P002", "RACAO-CAT-5KG", new Vector2(220, 320)));
        wh.Pallets.Add(new Pallet("P003", "RACAO-DOG-10KG", new Vector2(180, 260)));

        wh.Trucks.Add(new Truck("T-ABC1234", "D1"));
        
        wh.Docks[0].TruckId = "T-ABC1234";
        wh.Docks[0].HasTruckArrived = true;
        
        _bb.Data[$"notes:{wh.Trucks[0].Id}"] = new List<string> { "RACAO-DOG-10KG", "RACAO-CAT-5KG" };
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _dockTexture = Content.Load<Texture2D>("sprite_3");
        _truckTexture = Content.Load<Texture2D>("sprite_9");
        _emptyForkliftTexture = Content.Load<Texture2D>("sprite_12");
        _loadedForkliftTexture = Content.Load<Texture2D>("sprite_20");
        _palletTexture = Content.Load<Texture2D>("sprite_21");
        _palletTexture = Content.Load<Texture2D>("sprite_21");
        _greenTrafficLightTexture = Content.Load<Texture2D>("traffic_light_3");
        _yellowTrafficLightTexture = Content.Load<Texture2D>("traffic_light_1");
        _redTrafficLightTexture = Content.Load<Texture2D>("traffic_light_1");

    }

    protected override void Update(GameTime gameTime) 
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        foreach (var agent in _agents) 
            agent.Update(gameTime);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) 
    {
        GraphicsDevice.Clear(Color.LightGray);

        DisplayMode display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        var screenWidth  = display.Width;
        
        _spriteBatch.Begin();

        for (int i = 1; i <= 5; i++)
        {
            _spriteBatch.Draw(_dockTexture, new Vector2((screenWidth / 5f *i) -_dockTexture.Width / 5f, 0850), null, Color.White, MathHelper.ToRadians(0), new Vector2(_dockTexture.Width / 2f, _dockTexture.Height / 2f), 0.25f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(_truckTexture, new Vector2((screenWidth /5f *i) - _truckTexture.Width/ 5f - 140, 1000), null, Color.White, MathHelper.ToRadians(180), new Vector2(_truckTexture.Width / 2f, _truckTexture.Height / 2f), 0.2f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(_greenTrafficLightTexture, new Vector2((screenWidth / 5f *i) -_greenTrafficLightTexture.Width / 5f - 200, 0800), null, Color.White, MathHelper.ToRadians(0), new Vector2(_greenTrafficLightTexture.Width / 2f, _dockTexture.Height / 2f), 0.1f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(_emptyForkliftTexture, new Vector2((screenWidth /5f *i) - _emptyForkliftTexture.Width/ 5f - 160, 700), null, Color.White, MathHelper.ToRadians(180), new Vector2(_emptyForkliftTexture.Width / 2f, _emptyForkliftTexture.Height / 2f), 0.25f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(_loadedForkliftTexture, new Vector2((screenWidth /5f *i) - _loadedForkliftTexture.Width/ 5f - 160, 100), null, Color.White, MathHelper.ToRadians(0), new Vector2(_emptyForkliftTexture.Width / 2f, _emptyForkliftTexture.Height / 2f), 0.25f, SpriteEffects.None, 0f);

        }

        for (int i = 1; i <= 10; i++)
            for (int j = 1; j <= 10; j++)
                _spriteBatch.Draw(_palletTexture, new Vector2((screenWidth /10f * i) - _palletTexture.Width / 10f - 85, _palletTexture.Height / 4f * j), null, Color.White, MathHelper.ToRadians(0), new Vector2(_palletTexture.Width / 2f, _palletTexture.Height / 2f), 0.25f, SpriteEffects.None, 0f);
                       
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}