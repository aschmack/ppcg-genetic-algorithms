#include "./gamelogic.cpp"

//takes (len) bits of DNA as a binary number
int dnarange(dna_t &d, int start, int len) {
    int res = 0;
    for(int i = start; i < start+len; i++) {
        res = (res << 1) | d[i];
    }
    return res;
}

// get striped color score, 6 bits per color. should be
// resistant to getting erased by a crossover
void mapColorsBitwise(dna_t &d, int* color_array) {
	for (int i=0; i<N_COLORS; i++) {
		int score = 0;
		for (int j=0; j<6; j++) {
			score = (score<<1) | d[ j*N_COLORS + i ];
		}
		color_array[i] = score;
	}
}

// label for the lookup tables
enum direction_lut {
	UP_RIGHT=0, RIGHT, DOWN_RIGHT
};

// movement coord_t's to correspond to a direction
static const coord_t direction_lut[3] = {
	{ 1, -1 }, { 1, 0 }, { 1, 1 }
};

// indexes into the arrays to denote what should be summed
// for each direction.
static const int sum_lut[3][6] = {
	{ 3, 4, 8, 8, 9, 14 }, { 9, 13, 13, 14, 14, 19 },
	{ 14, 18, 18, 19, 23, 24 }
};

coord_t lookAheadPlayer(dna_t d, view_t v) {
	int scoreArray[25] = { 0 };
	int colorScores[N_COLORS] = { };
	
	// Get color mapping for this iteration
	mapColorsBitwise(d, colorScores);
		
	for (int y=-2; y<=2; y++) {
		for (int x=0; x<=2; x++) {
			// Get the scores for our whole field of view
			color_t color = v(x,y);
			if (color != OUT_OF_BOUNDS)
				scoreArray[ (x+2)+((y+2)*5) ] += colorScores[color];
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
    slog << "LookAheadPlayer score: " << runsimulation(lookAheadPlayer);
}