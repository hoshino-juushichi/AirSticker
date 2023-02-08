using System;
using AirSticker.Runtime.Scripts;
using AirSticker.Runtime.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Demo.Demo_00.Scripts
{
    public class AgingTest
    {
        private readonly Demo00 _demo00;
        private int _endIdleFrame;
        private int _frameCounter;

        private int _launchCount;

        private State _state = State.Idle;

        // Start is called before the first frame update
        public AgingTest(Demo00 demo00)
        {
            _demo00 = demo00;
        }

        // Update is called once per frame
        public void Update()
        {
            switch (_state)
            {
                case State.Idle:
                    Idle();

                    break;
                case State.ExecuteAction:
                    ExecuteAction();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Idle()
        {
            _frameCounter++;
            if (_frameCounter > _endIdleFrame) _state = State.ExecuteAction;
        }

        private void ExecuteAction()
        {
            if (AirStickerSystem.DecalProjectorLauncher.GetWaitingRequestCount() > 50)
            {
                // The request for projection decal is over 50, so return.
                TrianglePolygonsFactory.MaxGeneratedPolygonPerFrame = 100;
                return;
            }

            if (_launchCount < 40)
            {
                // Randomly determined position.
                var pos = new Vector2();
                pos.x = Screen.width * 0.5f;
                pos.y = Screen.height * 0.5f;
                var widthHalfRange = Screen.width * 0.2f;
                var heightHalfRange = Screen.height * 0.5f;
                pos.x += Random.Range(-widthHalfRange, widthHalfRange);
                pos.y += Random.Range(-heightHalfRange, heightHalfRange);
                _demo00.Launch(
                    pos,
                    Random.Range(0, 6));
                TrianglePolygonsFactory.MaxGeneratedPolygonPerFrame = Random.Range(50, 100);
                _launchCount++;
                _frameCounter = 0;
                _endIdleFrame = Random.Range(0, 20);
            }
            else
            {
                //　Lottery for next action.
                var t = Random.Range(0, 99);
                if (t < 20)
                {
                    _demo00.DeleteCurrentReceiverObject();
                    _demo00.SetNextReceiverObject();
                    if (_demo00.HasReceiverObjectsDeleted())
                        // Reset scene.
                        SceneManager.LoadScene("Demo_00");
                }
                else if (t < 40)
                {
                    SceneManager.LoadScene("Demo_00");
                }

                _launchCount = 0;
                _frameCounter = 0;
                _endIdleFrame = 0;
            }

            _state = State.Idle;
        }

        private enum State
        {
            Idle,
            ExecuteAction
        }
    }
}
