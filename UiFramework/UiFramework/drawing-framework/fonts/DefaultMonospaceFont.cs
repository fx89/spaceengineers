using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.drawing_framework.fonts {
    public class DefaultMonospaceFont {
        private const bool O = true;
        private const bool _ = false;

        private const int DEFAULT_FONT_HEIGHT = 7;

        private static MySprite SPRITE_A = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,O,O,O,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_
        });

        private static MySprite SPRITE_B = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,O,O,O,_,_
        });

        private static MySprite SPRITE_C = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,O,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            _,O,O,O,O,_
        });

        private static MySprite SPRITE_D = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,_,_,_,
            O,_,_,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,O,_,_,
            O,O,O,_,_,_
        });

        private static MySprite SPRITE_E = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,O,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,O,O,O,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,O,O,O,O,_
        });

        private static MySprite SPRITE_F = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,O,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,O,O,O,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_
        });

        private static MySprite SPRITE_G = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,O,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,O,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,O,_
        });

        private static MySprite SPRITE_H = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,O,O,O,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_
        });

        private static MySprite SPRITE_I = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,O,O,O,_,_
        });

        private static MySprite SPRITE_J = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,_,_,O,_,_,
            _,_,_,O,_,_,
            _,_,_,O,_,_,
            _,_,_,O,_,_,
            _,_,_,O,_,_,
            _,_,_,O,_,_,
            _,O,O,_,_,_
        });

        private static MySprite SPRITE_K = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,_,_,O,_,_,
            O,_,O,_,_,_,
            O,O,_,_,_,_,
            O,_,O,_,_,_,
            O,_,_,O,_,_,
            O,_,_,_,O,_
        });

        private static MySprite SPRITE_L = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            _,O,O,O,O,_
        });

        private static MySprite SPRITE_M = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,O,_,O,O,_,
            O,_,O,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_
        });

        private static MySprite SPRITE_N = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,O,_,_,O,_,
            O,_,O,_,O,_,
            O,_,_,O,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_
        });

        private static MySprite SPRITE_O = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,_,_
        });

        private static MySprite SPRITE_P = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,O,O,O,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_
        });

        private static MySprite SPRITE_Q = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,O,_,O,_,
            O,_,_,O,_,_,
            _,O,O,_,O,_
        });

        private static MySprite SPRITE_R = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,O,O,O,_,_,
            O,_,O,_,_,_,
            O,_,_,O,_,_,
            O,_,_,_,O,_
        });

        private static MySprite SPRITE_S = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,O,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            _,O,O,O,_,_,
            _,_,_,_,O,_,
            _,_,_,_,O,_,
            O,O,O,O,_,_
        });

        private static MySprite SPRITE_T = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,O,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_
        });

        private static MySprite SPRITE_U = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,_,_
        });

        private static MySprite SPRITE_V = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,_,O,_,_,
            _,_,O,_,_,_
        });

        private static MySprite SPRITE_W = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            O,_,O,_,O,_,
            _,O,_,O,_,_
        });

        private static MySprite SPRITE_X = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,_,O,_,_,
            _,_,O,_,_,_,
            _,O,_,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_
        });

        private static MySprite SPRITE_Y = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,_,O,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_,
            _,_,O,_,_,_
        });

        private static MySprite SPRITE_Z = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,O,_,
            _,_,_,_,O,_,
            _,_,_,O,_,_,
            _,_,O,_,_,_,
            _,O,_,_,_,_,
            O,_,_,_,_,_,
            O,O,O,O,O,_
        });

        private static MySprite SPRITE_1 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,_,_,O,_,_,
            _,_,O,O,_,_,
            _,O,_,O,_,_,
            _,_,_,O,_,_,
            _,_,_,O,_,_,
            _,_,_,O,_,_,
            _,_,O,O,O,_
        });

        private static MySprite SPRITE_2 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,_,O,O,_,_,
            _,O,_,_,O,_,
            _,_,_,_,O,_,
            _,_,_,_,O,_,
            _,O,O,O,_,_,
            _,O,_,_,_,_,
            _,O,O,O,O,_
        });

        private static MySprite SPRITE_3 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,_,O,O,_,_,
            _,O,_,_,O,_,
            _,_,_,_,O,_,
            _,_,O,O,_,_,
            _,_,_,_,O,_,
            _,O,_,_,O,_,
            _,_,O,O,_,_
        });

        private static MySprite SPRITE_4 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,_,_,O,_,_,
            _,_,O,O,_,_,
            _,O,_,O,_,_,
            O,_,_,O,_,_,
            O,O,O,O,O,_,
            _,_,_,O,_,_,
            _,_,_,O,_,_
        });

        private static MySprite SPRITE_5 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,O,_,
            _,O,_,_,_,_,
            _,O,_,_,_,_,
            _,O,O,O,_,_,
            _,_,_,_,O,_,
            _,_,_,_,O,_,
            _,O,O,O,_,_
        });

        private static MySprite SPRITE_6 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,O,_,
            O,_,_,_,_,_,
            O,_,_,_,_,_,
            O,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,_,_
        });

        private static MySprite SPRITE_7 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            O,O,O,O,O,_,
            _,_,_,_,O,_,
            _,_,_,_,O,_,
            _,_,_,O,_,_,
            _,_,O,_,_,_,
            _,O,_,_,_,_,
            _,O,_,_,_,_
        });

        private static MySprite SPRITE_8 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,_,_
        });

        private static MySprite SPRITE_9 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,O,_,
            _,_,_,_,O,_,
            _,_,_,_,O,_,
            O,O,O,O,_,_
        });

        private static MySprite SPRITE_0 = new MySprite(6, DEFAULT_FONT_HEIGHT, new bool[] {
            _,O,O,O,_,_,
            O,_,_,_,O,_,
            O,_,_,O,O,_,
            O,_,O,_,O,_,
            O,O,_,_,O,_,
            O,_,_,_,O,_,
            _,O,O,O,_,_
        });

        private static MySprite[] Create() {
	        MySprite[] BitmapFont = new MySprite[256];

	        BitmapFont['a'] = SPRITE_A; BitmapFont['A'] = SPRITE_A;
	        BitmapFont['b'] = SPRITE_B; BitmapFont['B'] = SPRITE_B;
	        BitmapFont['c'] = SPRITE_C; BitmapFont['C'] = SPRITE_C;
	        BitmapFont['d'] = SPRITE_D; BitmapFont['D'] = SPRITE_D;
	        BitmapFont['e'] = SPRITE_E; BitmapFont['E'] = SPRITE_E;
	        BitmapFont['f'] = SPRITE_F; BitmapFont['F'] = SPRITE_F;
	        BitmapFont['g'] = SPRITE_G; BitmapFont['G'] = SPRITE_G;
	        BitmapFont['h'] = SPRITE_H; BitmapFont['H'] = SPRITE_H;
	        BitmapFont['i'] = SPRITE_I; BitmapFont['I'] = SPRITE_I;
	        BitmapFont['j'] = SPRITE_J; BitmapFont['J'] = SPRITE_J;
	        BitmapFont['k'] = SPRITE_K; BitmapFont['K'] = SPRITE_K;
	        BitmapFont['l'] = SPRITE_L; BitmapFont['L'] = SPRITE_L;
	        BitmapFont['m'] = SPRITE_M; BitmapFont['M'] = SPRITE_M;
	        BitmapFont['n'] = SPRITE_N; BitmapFont['N'] = SPRITE_N;
	        BitmapFont['o'] = SPRITE_O; BitmapFont['O'] = SPRITE_O;
	        BitmapFont['p'] = SPRITE_P; BitmapFont['P'] = SPRITE_P;
	        BitmapFont['q'] = SPRITE_Q; BitmapFont['Q'] = SPRITE_Q;
	        BitmapFont['r'] = SPRITE_R; BitmapFont['R'] = SPRITE_R;
	        BitmapFont['s'] = SPRITE_S; BitmapFont['S'] = SPRITE_S;
	        BitmapFont['t'] = SPRITE_T; BitmapFont['T'] = SPRITE_T;
	        BitmapFont['u'] = SPRITE_U; BitmapFont['U'] = SPRITE_U;
	        BitmapFont['v'] = SPRITE_V; BitmapFont['V'] = SPRITE_V;
	        BitmapFont['w'] = SPRITE_W; BitmapFont['W'] = SPRITE_W;
	        BitmapFont['x'] = SPRITE_X; BitmapFont['X'] = SPRITE_X;
	        BitmapFont['y'] = SPRITE_Y; BitmapFont['Y'] = SPRITE_Y;
	        BitmapFont['z'] = SPRITE_Z; BitmapFont['Z'] = SPRITE_Z;
	        BitmapFont['1'] = SPRITE_1;
	        BitmapFont['2'] = SPRITE_2;
	        BitmapFont['3'] = SPRITE_3;
	        BitmapFont['4'] = SPRITE_4;
	        BitmapFont['5'] = SPRITE_5;
	        BitmapFont['6'] = SPRITE_6;
	        BitmapFont['7'] = SPRITE_7;
	        BitmapFont['8'] = SPRITE_8;
	        BitmapFont['9'] = SPRITE_9;
	        BitmapFont['0'] = SPRITE_0;

	        return BitmapFont;
        }

        public static MySprite[] BitmapFont = Create();
    }
}
