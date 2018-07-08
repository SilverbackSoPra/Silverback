using System;
using System.Collections.Generic;
using LevelEditor.Collision;
using LevelEditor.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AStartTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            var startVertex = new Vertex(new Vector2(7, 11),
                new CollisionRectangle(new Vector2(-1, 1), new Vector2(1, 1), new Vector2(-1, -1)));

            var v1 = new Vertex(new Vector2(5, 13),
                new CollisionRectangle(new Vector2(4, 6), new Vector2(6, 6), new Vector2(4, 4)));

            var v2 = new Vertex(new Vector2(2, 10),
                new CollisionRectangle(new Vector2(4, 6), new Vector2(6, 6), new Vector2(4, 4)));

            var v3 = new Vertex(new Vector2(3, 5),
                new CollisionRectangle(new Vector2(4, 6), new Vector2(6, 6), new Vector2(4, 4)));

            var v4 = new Vertex(new Vector2(6, 7),
                new CollisionRectangle(new Vector2(4, 6), new Vector2(6, 6), new Vector2(4, 4)));

            var v5 = new Vertex(new Vector2(11, 2),
                new CollisionRectangle(new Vector2(4, 6), new Vector2(6, 6), new Vector2(4, 4)));

            var v6 = new Vertex(new Vector2(13, 10),
                new CollisionRectangle(new Vector2(4, 6), new Vector2(6, 6), new Vector2(4, 4)));

            var destinationVertex = new Vertex(new Vector2(15, 6),
                new CollisionRectangle(new Vector2(4, 6), new Vector2(6, 6), new Vector2(4, 4)));

            var e1 = new Edge(startVertex, v1, 10);
            var e2 = new Edge(v1, v2, 10);
            var e3 = new Edge(v2, v3, 10);
            var e4 = new Edge(v3, v4, 10);
            var e5 = new Edge(v4, v5, 10);
            var e6= new Edge(v5, v6, 10);
            var e7 = new Edge(v6, destinationVertex, 10);
            var e8 = new Edge(v2, v5, 10);
            var e9 = new Edge(v1, v3, 10);
            var e10 = new Edge(v3, destinationVertex, 10);

            var vertices = new List<Vertex>() { startVertex, v1, v2, v3, v4, v5, v6};
            // var edgeList = new List<Edge>() { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10};
            var edgeList = new List<Edge>() { e1, e2, e3, e4, e5, e6, e7, new Edge(startVertex, v6, 10) };

            var vertexNode = LevelEditor.Pathfinding.AStar.Search(
                startVertex,
                destinationVertex,
                vertices,
                edgeList,
                (vertex, edge) =>
                {
                    if (edge.V1 == vertex)
                    {
                        return edge.V2;
                    }

                    if (edge.V2 == vertex)
                    {
                        return edge.V1;
                    }

                    return default(Vertex);
                });

            while (vertexNode.mNextVertex != null)
            {
                Console.WriteLine("Vertex at Position " + vertexNode.Position + " --> Vertex at " + vertexNode.mNextVertex.Position + " with a cost of " + vertexNode.mNextVertex.mCost);
                vertexNode = vertexNode.mNextVertex;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
