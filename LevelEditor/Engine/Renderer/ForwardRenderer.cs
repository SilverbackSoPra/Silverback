using LevelEditor.Engine.Shader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LevelEditor.Engine.Renderer
{
    internal sealed class ForwardRenderer : IRenderer
    {

        public bool PrePass { private get; set; }

        private readonly ForwardShader mShader;
        private readonly ForwardShader mShaderSkinned;
        private readonly ForwardShader mShaderTerrain;
        private readonly GraphicsDevice mGraphicsDevice;
        private readonly DepthStencilState mDepthDisabled;
        private bool mDisposed = false;


        private static readonly SamplerState sShadowMap = new SamplerState
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = TextureFilter.Linear,
            ComparisonFunction = CompareFunction.LessEqual,
            FilterMode = TextureFilterMode.Comparison
        };

        public ForwardRenderer(GraphicsDevice device, ContentManager content, string shaderPath, string shaderPathSkinned, string shaderPathTerrain)
        {

            mShader = new ForwardShader(content, shaderPath);
            mShaderSkinned = new ForwardShader(content, shaderPathSkinned);
            mShaderTerrain = new ForwardShader(content, shaderPathTerrain);

            mDepthDisabled = new DepthStencilState
            {
                StencilEnable = false,
                DepthBufferFunction = CompareFunction.LessEqual,
                DepthBufferWriteEnable = false
            };

            mGraphicsDevice = device;

        }

        public void Render(RenderTarget target, Camera camera, Scene scene)
        {

            mGraphicsDevice.SamplerStates[1] = sShadowMap;
            var defaultState = mGraphicsDevice.DepthStencilState;

            if (PrePass)
            {

                // We first render the skinned meshes
                Render(target, camera, scene, mShaderSkinned, true, false);
                mGraphicsDevice.DepthStencilState = mDepthDisabled;
                // Then we render the unskinned meshes
                Render(target, camera, scene, mShader, false, false);
                // Render terrain at last
                Render(target, camera, scene, mShaderTerrain, false, true);
                mGraphicsDevice.DepthStencilState = defaultState;

            }
            else
            {

                // We first render the unskinned meshes
                Render(target, camera, scene, mShader, false, false);
                // Render terrain at last
                Render(target, camera, scene, mShaderTerrain, false, true);
                // Then we render the skinned meshes
                Render(target, camera, scene, mShaderSkinned, true, false);

            }


        }

        private void Render(RenderTarget target, Camera camera, Scene scene, ForwardShader shader, bool skinnedRendering, bool terrainRendering)
        {

            var light = scene.mSky.Light;

            shader.mViewMatrix = camera.mViewMatrix;
            shader.mProjectionMatrix = camera.mProjectionMatrix;
            shader.mFarPlane = camera.mFarPlane;

            var upVector = Vector4.Transform(new Vector4(0.0f, 1.0f, 0.0f, 0.0f), camera.mViewMatrix);
            shader.mUp = new Vector3(upVector.X, upVector.Y, upVector.Z);

            // We need the light location to be in view space (because the shader does all operations in view space)
            shader.mGlobalLightDirection = Vector3.Normalize(Vector3.Transform(light.mLocation, camera.mViewMatrix) - Vector3.Transform(Vector3.Zero, camera.mViewMatrix));
            shader.mGlobalLightColor = light.mColor;
            shader.mGlobalLightAmbient = light.mAmbient;

            if (light.mShadow.mActivated)
            {
                shader.mLightSpaceMatrix = Matrix.Invert(camera.mViewMatrix) * light.mShadow.mViewMatrix *
                                           light.mShadow.mProjectionMatrix;
                shader.mShadowMap = target.mShadowRenderTarget;
                shader.mShadowBias = light.mShadow.mBias;
                shader.mShadowNumSamples = Math.Min(light.mShadow.mNumSamples, 16);
                shader.mShadowSampleRange = light.mShadow.mSampleRange;
                shader.mShadowDistance = light.mShadow.mDistance;
                shader.mShadowResolution = target.mShadowRenderTarget.Width;
            }
            else
            {
                shader.mShadowNumSamples = 0;
            }

            shader.mFogColor = scene.mFog.mColor;
            shader.mFogDistance = Math.Min(scene.mFog.mDistance, camera.mFarPlane - 10.0f);

            shader.Apply();

            // Now render our actor in batched mode
            foreach (var actorBatch in scene.mActorBatches)
            {

                var meshData = actorBatch.mMesh.mMeshData;

                // When using the unskinned shader we don't want to render skinned meshes
                if (meshData.mIsSkinned && !skinnedRendering || meshData.mIsTerrain && !terrainRendering)
                {
                    continue;
                }

                // When using the skinned shader we don't want to render unskinned meshes
                if (!meshData.mIsSkinned && skinnedRendering || !meshData.mIsTerrain && terrainRendering)
                {
                    continue;
                }

                mGraphicsDevice.SetVertexBuffer(actorBatch.mMesh.VertexBuffer);
                mGraphicsDevice.Indices = actorBatch.mMesh.IndexBuffer;

                foreach (var subData in meshData.mSubDatas)
                {

                    var material = meshData.mMaterials[subData.mMaterialIndex];

                    // Apply all material properties
                    shader.ApplyMaterial(material);

                    foreach (var actor in actorBatch.mActors)
                    {

                        if (!actor.mRender)
                        {
                            continue;
                        }

                        shader.ApplyActor(actor.ModelMatrix, actor.Color);

                        if (skinnedRendering)
                        {
                            shader.ApplyBoneTransformations(actor.mAnimator.mTransformations);
                        }

                        mGraphicsDevice.DrawIndexedPrimitives(meshData.mPrimitiveType,
                            0,
                            subData.mIndicesOffset,
                            subData.mNumPrimitives);

                    }

                }

            }

        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {

            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                mDepthDisabled.Dispose();
            }

            mDisposed = true;

        }

    }

}
