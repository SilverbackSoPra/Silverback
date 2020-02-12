using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LevelEditor.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Newtonsoft.Json.Linq;

namespace LevelEditor.Engine
{
    /// <summary>
    /// Used to represent a camera in the engine.
    /// In third person mode the camera location represents the point the camera looks at while
    /// in first person mode the camera is at the actual location.
    /// </summary>
    public sealed class Camera: ISaver, ILoader
    {

        public Vector3 mLocation;
        public Vector2 mRotation;

        public float mFieldOfView;
        public float mAspectRatio;
        public float mNearPlane;
        public float mFarPlane;

        public bool mThirdPerson;
        public float mThirdPersonDistance;

        public Matrix mViewMatrix;
        public Matrix mProjectionMatrix;

        private readonly AudioListener mListener;

        public Vector3 Direction { get; set; }
        
        public Vector3 Up { get; set; }

        public Vector3 Right { get; set; }

        /// <summary>
        /// Constructs a <see cref="Camera"/>.
        /// </summary>
        /// <param name="location">The location of the camera.</param>
        /// <param name="rotation">The rotation of the camera, where .X is the horizontal and .Y the vertical rotation.</param>
        /// <param name="fieldOfView">The field of view in degrees.</param>
        /// <param name="aspectRatio">The ratio of the image width to the image height.</param>
        /// <param name="nearPlane">The plane where the camera starts to render.</param>
        /// <param name="farPlane">The plane where the camera stops to render.</param>
        /// <param name="thirdPerson">If set to true a third person camera will be calculated</param>
        /// <param name="thirdPersonDistance">Sets the distance for third person camera</param>
        public Camera(Vector3 location = default(Vector3),
            Vector2 rotation = default(Vector2),
            float fieldOfView = 45.0f,
            float aspectRatio = 2.0f,
            float nearPlane = 1.0f,
            float farPlane = 100.0f,
            bool thirdPerson = false,
            float thirdPersonDistance = 10.0f)
        {
            mViewMatrix = new Matrix();
            mProjectionMatrix = new Matrix();

            mLocation = location;
            mRotation = rotation;

            mFieldOfView = fieldOfView;
            mAspectRatio = aspectRatio;

            mNearPlane = nearPlane;
            mFarPlane = farPlane;

            mThirdPerson = thirdPerson;
            mThirdPersonDistance = thirdPersonDistance;

            mListener = new AudioListener();

        }

        /// <summary>
        /// Calculates the view matrix based on the location and rotation of the camera.
        /// </summary>
        public void UpdateView()
        {
            
            Direction = -Vector3.Normalize(new Vector3((float) (Math.Cos(mRotation.Y) * Math.Sin(mRotation.X)),
                (float) Math.Sin(mRotation.Y),
                (float) (Math.Cos(mRotation.Y) * Math.Cos(mRotation.X))));

            Right = -Vector3.Normalize(new Vector3((float) Math.Sin(mRotation.X - 3.14 / 2.0), 0.0f, (float) Math.Cos(mRotation.X - 3.14 / 2.0)));

            if (mThirdPerson)
            {
                Up = Vector3.Cross(Right, -Direction);
                mViewMatrix = Matrix.CreateLookAt(mLocation - Direction * mThirdPersonDistance, mLocation, -Up);
            }
            else
            {
                Up = Vector3.Cross(Right, Direction);
                mViewMatrix = Matrix.CreateLookAt(mLocation, mLocation + Direction, Up);
            }

            var translation = mViewMatrix.Translation;
            mListener.Position = translation;
            mListener.Velocity = Vector3.Zero;
            mListener.Forward = Direction;
            mListener.Up = Up;
            
        }

        /// <summary>
        /// Calculates the perspective matrix based on the FoV, the aspect ratio and the near and far plane.
        /// </summary>
        public void UpdatePerspective()
        {
            mProjectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(mFieldOfView / 180.0f * (float)Math.PI, mAspectRatio, mNearPlane, mFarPlane);
        }

        public string Save()
        {
            
            var str = "{" +
                        "\"mLocation\": {" +
                            "\"X\": \"" + mLocation.X + "\"," +
                            "\"Y\": \"" + mLocation.Y + "\"," +
                            "\"Z\": \"" + mLocation.Z + "\"" +
                        "}," +
                        "\"mRotation\": {" +
                            "\"X\": \"" + mRotation.X + "\"," +
                            "\"Y\": \"" + mRotation.Y + "\"" +
                        "}," +
                        "\"mFieldOfView\": \"" + mFieldOfView + "\"," +
                        "\"mAspectRatio\": \"" + mAspectRatio + "\"," +
                        "\"mNearPlane\": \"" + mNearPlane + "\"," +
                        "\"mThirdPerson\": \"" + mThirdPerson + "\"," +
                        "\"mThirdPersonDistance\": \"" + mThirdPersonDistance + "\"," +
                        "\"Direction\": {" +
                            "\"X\": \"" + Direction.X + "\"," +
                            "\"Y\": \"" + Direction.Y + "\"," +
                            "\"Z\": \"" + Direction.Z + "\"" +
                        "}," +
                        "\"Up\": {" +
                            "\"X\": \"" + Up.X + "\"," +
                            "\"Y\": \"" + Up.Y + "\"," +
                            "\"Z\": \"" + Up.Z + "\"" +
                        "}," +
                        "\"Right\": {" +
                            "\"X\": \"" + Right.X + "\"," +
                            "\"Y\": \"" + Right.Y + "\"," +
                            "\"Z\": \"" + Right.Z + "\"" +
                        "}" +
                      "}";
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public bool Load(string str)
        {
            var c = Newtonsoft.Json.JsonConvert.DeserializeObject<Camera>(str);
            float t = c.mFieldOfView;// ["mFieldOfView"];
            Console.WriteLine(t);

            /*var objects = JArray.Parse(str); // parse as array  
            foreach (var jToken in objects)
            {
                var root = (JObject) jToken;
                foreach (var key in root)
                {
                    switch (key.Key)
                    {
                        case "mFieldOfView":
                            Console.WriteLine(key.Key);
                            Console.WriteLine(key.Value);
                            Console.WriteLine("\n");
                            break;
                        default:
                            break;
                            
                    }
                    // var description = (string)key.Value["Description"];
                    // var value = (string)key.Value["Value"];
                }
            }*/
            return false;
        }

        private Camera()
        {
            mListener = new AudioListener();
        }
    }
}