
using UnityEngine;
using UnityEngine.UI;

namespace game.ui
{
    public enum CheatState
    {
        NONE,
        PENTOMINO,
        GLIDER,
        ACORN,
    }

    public class CheatMenuHook : MonoBehaviour
    {
        
        public delegate void changeCheatState(CheatState state, int[] figure);
        public Canvas canvasHook;
        private CheatState _state = CheatState.NONE;
        private changeCheatState _onChangeState;
        
        private int[] pento;
        private int[] glider;
        private int[] acorn;
        
        public void toggleContent()
        {
            canvasHook.enabled = !canvasHook.enabled;
        }
        
        public void hideContent()
        {
            canvasHook.enabled = false;
        }
        
        public void showContent()
        {
            canvasHook.enabled = true;
        }
        
        public void initialize(int w, int h, changeCheatState onChangeState)
        {
            _onChangeState = onChangeState;
            pento = new int[10];
            pento[0] = 1; pento[1] = 0;
            pento[2] = 2; pento[3] = 0;
            pento[4] = 0; pento[5] = 1;
            pento[6] = 1; pento[7] = 1;
            pento[8] = 2; pento[9] = 1;
        }
        
        public void onPentomino()
        {
            if (_state == CheatState.PENTOMINO)
                _state = CheatState.NONE;
            else
                _state = CheatState.PENTOMINO;
            _onChangeState(_state, pento);
        }
        
        public void onGlider()
        {
            if (_state == CheatState.GLIDER)
                _state = CheatState.NONE;
            else
                _state = CheatState.GLIDER;
            _onChangeState(_state, glider);
        }
        
        public void OnAcort()
        {
            if (_state == CheatState.ACORN)
                _state = CheatState.NONE;
            else
                _state = CheatState.ACORN;
            _onChangeState(_state, acorn);
        }
    }
}
