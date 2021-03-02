using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.drawing_framework.fonts {
    public class DefaultMonospaceFont {
        private static MySprite CreateFontSprite(byte[] bytes) {
         // Cut a couple of pixels off the right edge
            MyCanvas Cvs = new MyCanvas(6, 7);
            Cvs.BitBlt(new MySprite(8, 7, DrawingFrameworkUtils.ByteArrayToBoolArray(bytes)), 0, 0);
            return new MySprite(6, 7, Cvs.GetBuffer());
        }

        private static MySprite SPRITE_A = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0xf8,0x88,0x88,0x88 });
        private static MySprite SPRITE_B = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x88,0x88,0xf0 });
        private static MySprite SPRITE_C = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x80,0x80,0x80,0x78 });
        private static MySprite SPRITE_D = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0x88,0x88,0x88,0xf0 });
        private static MySprite SPRITE_E = CreateFontSprite(new byte[] { 0xf0,0x80,0x80,0xf0,0x80,0x80,0xf0 });
        private static MySprite SPRITE_F = CreateFontSprite(new byte[] { 0xf8,0x80,0x80,0xf0,0x80,0x80,0x80 });
        private static MySprite SPRITE_G = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x98,0x88,0x88,0x78 });
        private static MySprite SPRITE_H = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0xf8,0x88,0x88,0x88 });
        private static MySprite SPRITE_I = CreateFontSprite(new byte[] { 0x70,0x20,0x20,0x20,0x20,0x20,0x70 });
        private static MySprite SPRITE_J = CreateFontSprite(new byte[] { 0x10,0x10,0x10,0x10,0x10,0x10,0x60 });
        private static MySprite SPRITE_K = CreateFontSprite(new byte[] { 0x88,0x90,0xa0,0xc0,0xa0,0x90,0x88 });
        private static MySprite SPRITE_L = CreateFontSprite(new byte[] { 0x80,0x80,0x80,0x80,0x80,0x80,0x78 });
        private static MySprite SPRITE_M = CreateFontSprite(new byte[] { 0x88,0xd8,0xa8,0x88,0x88,0x88,0x88 });
        private static MySprite SPRITE_N = CreateFontSprite(new byte[] { 0x88,0xc8,0xa8,0x98,0x88,0x88,0x88 });
        private static MySprite SPRITE_O = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0x88,0x88,0x70 });
        private static MySprite SPRITE_P = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x80,0x80,0x80 });
        private static MySprite SPRITE_Q = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0xa8,0x90,0x68 });
        private static MySprite SPRITE_R = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0xa0,0x90,0x88 });
        private static MySprite SPRITE_S = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x70,0x08,0x08,0xf0 });
        private static MySprite SPRITE_T = CreateFontSprite(new byte[] { 0xf8,0x20,0x20,0x20,0x20,0x20,0x20 });
        private static MySprite SPRITE_U = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x88,0x70 });
        private static MySprite SPRITE_V = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x50,0x20 });
        private static MySprite SPRITE_W = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0xa8,0x50 });
        private static MySprite SPRITE_X = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x50,0x88,0x88 });
        private static MySprite SPRITE_Y = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x20,0x20,0x20 });
        private static MySprite SPRITE_Z = CreateFontSprite(new byte[] { 0xf8,0x08,0x10,0x20,0x40,0x80,0xf8 });
        private static MySprite SPRITE_1 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x10,0x10,0x10,0x38 });
        private static MySprite SPRITE_2 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x08,0x70,0x40,0x78 });
        private static MySprite SPRITE_3 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x30,0x08,0x48,0x30 });
        private static MySprite SPRITE_4 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x90,0xf8,0x10,0x10 });
        private static MySprite SPRITE_5 = CreateFontSprite(new byte[] { 0x78,0x40,0x40,0x70,0x08,0x08,0x70 });
        private static MySprite SPRITE_6 = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0xf0,0x88,0x88,0x70 });
        private static MySprite SPRITE_7 = CreateFontSprite(new byte[] { 0xf8,0x08,0x08,0x10,0x20,0x40,0x40 });
        private static MySprite SPRITE_8 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x88,0x88,0x70 });
        private static MySprite SPRITE_9 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x08,0x08,0xf0 });
        private static MySprite SPRITE_0 = CreateFontSprite(new byte[] { 0x70,0x88,0x98,0xa8,0xc8,0x88,0x70 });

        private static MySprite SPRITE_DASH   = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0x00,0x00,0x00,0xf8 });
        private static MySprite SPRITE_HYPHEN = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0xf8,0x00,0x00,0x00 });
        private static MySprite SPRITE_GT     = CreateFontSprite(new byte[] { 0x40,0x20,0x10,0x08,0x10,0x20,0x40 });
        private static MySprite SPRITE_LT     = CreateFontSprite(new byte[] { 0x08,0x10,0x20,0x40,0x20,0x10,0x08 });
        private static MySprite SPRITE_EQ     = CreateFontSprite(new byte[] { 0x00,0x00,0xf8,0x00,0xf8,0x00,0x00 });
        private static MySprite SPRITE_PCT    = CreateFontSprite(new byte[] { 0xc0,0xc8,0x10,0x20,0x40,0x98,0x18 });

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
            BitmapFont['_'] = SPRITE_DASH;
            BitmapFont['-'] = SPRITE_HYPHEN;
            BitmapFont['<'] = SPRITE_LT;
            BitmapFont['>'] = SPRITE_GT;
            BitmapFont['='] = SPRITE_EQ;
            BitmapFont['%'] = SPRITE_PCT;

            return BitmapFont;
        }

        public static MySprite[] BitmapFont = Create();
    }
}
