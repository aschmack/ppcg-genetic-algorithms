#include "./gamelogic.cpp"
#include <string.h>
#include <fstream>

#define ID_BITS 4
#define SCORE_BITS 3
#define TOTAL_BITS (ID_BITS+SCORE_BITS)
#define MAX_SCORE ((int)pow(SCORE_BITS,2)-1)

// label that may not be used
enum direction_lut {
	UP_RIGHT=0, RIGHT, DOWN_RIGHT
};

// score index arrays for the 8 movement directions
static const int sum_lut[3][6] = {
	{ 2, 3, 4, 8, 9, 14 }, { 8, 9, 13, 14, 18, 19 },
	{ 14, 18, 19, 22, 23, 24 }
};

// return values corresponding to the movement directions
static const coord_t direction_lut[3] = {
	{ 1, -1 }, { 1, 0 }, { 1, 1 }
};

//takes (len) bits of DNA as a binary number
int dnarange(dna_t &d, int start, int len) {
    int res = 0;
    for(int i = start; i < start+len; i++) {
        res = (res << 1) | d[i];
    }
    return res;
}

//go through our "knowledge" and make a map from color->score for
//quick lookup
void mapColors(dna_t &d, int* color_array=NULL) {
	for (int bits = 0; bits < DNA_BITS-TOTAL_BITS; bits += TOTAL_BITS) {
		int color_id = dnarange(d,bits,ID_BITS);
		int color_knowledge = (int)dnarange(d,bits+ID_BITS,SCORE_BITS);
		
		// If it falls on an odd bit then it is negative knowledge
		if ( (bits+ID_BITS)%2 == 0 )
			color_array[color_id]+=color_knowledge;
		else
			color_array[color_id]-=color_knowledge;
	}
}

coord_t lookAheadPlayer(dna_t d, view_t v) {
	int scoreArray[25] = { 0 };
	int seenColors[N_COLORS] = { };
	mapColors(d, seenColors);

		
	for (int y=-2; y<=2; y++) {
		for (int x=0; x<=2; x++) {
			// Get the scores for our whole field of view
			color_t color = v(x,y);
			if (color != OUT_OF_BOUNDS)
				scoreArray[ (x+2)+((y+2)*5) ] += seenColors[color];
		}
	}
	
	// get the best move by summing all of the array indices for a particular
	// direction
	int best = RIGHT;
	int bestScore = 0;
	for (int dir=UP_RIGHT; dir<=DOWN_RIGHT; dir++) {
		if (v(direction_lut[dir].x, direction_lut[dir].y) == OUT_OF_BOUNDS)
			continue;
		
		int score = 0;
		for (int i=0; i<6; i++) {
			score += scoreArray[ sum_lut[dir][i] ];
		}
		
		if (score > bestScore) {
			bestScore = score;
			best = dir;
		}
	}
	
	return direction_lut[best];
}

int main() {
    slog << "Look ahead score: " << runsimulation(lookAheadPlayer);
}