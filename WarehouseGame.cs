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
    private Texture2D _backgroundTexture;
    private Texture2D _forkliftTexture;
    private Texture2D _palletTexture;
    private Texture2D _truckTexture;

    public WarehouseGame() 
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
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

    private void SeedWorld(Warehouse wh) {
        wh.Docks.Add(new Dock("D1", new Vector2(600, 200)));
        wh.Pallets.Add(new Pallet("P001", "RACAO-DOG-10KG", new Vector2(120, 300)));
        wh.Pallets.Add(new Pallet("P002", "RACAO-CAT-5KG", new Vector2(220, 320)));
        wh.Pallets.Add(new Pallet("P003", "RACAO-DOG-10KG", new Vector2(180, 260)));
        wh.Trucks.Add(new Truck("T-ABC1234", "D1"));
        wh.Docks[0].TruckId = "T-ABC1234";
        wh.Docks[0].HasTruckArrived = true;

        // ERP “database”
        _bb.Data[$"notes:{wh.Trucks[0].Id}"] = new List<string> { "RACAO-DOG-10KG", "RACAO-CAT-5KG" };
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _forkliftTexture = Content.Load<Texture2D>("sprite_20");        
        _palletTexture = Content.Load<Texture2D>("sprite_21");
        _truckTexture = Content.Load<Texture2D>("sprite_9");
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
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        
        /*
        _spriteBatch.Draw(
            _forkliftTexture,
            new Vector2(100, 100),        // posição
            null,                     // source rectangle (null = toda a imagem)
            Color.White,              // cor
            MathHelper.ToRadians(90),                       // rotação
            Vector2.Zero,             // origem
            0.5f,                     // escala (50% do tamanho original)
            SpriteEffects.None,
            0f
        );
        */

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}