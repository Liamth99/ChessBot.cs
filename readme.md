# Very early WIP

### Inspired by [Sebastian Lague's](https://github.com/SebLague/Chess-Coding-Adventure) Videos

My personal goal is to learn more about new types of programming and thought I'd start 
with a C# Chess bot as I'm familiar with the language and then write another bot in C, 
C++ or Rust. With the hope that more Chess programming experience and the **"performance advantage"** 
of the lower languages, I can beat the C# one.

And then maybe in the future ill make one in a functional language or use a Neural network or some other dumb idea.

## Few Self Imposed Rules
 - No stealing other peoples code or using AI. The whole point is to learn (I will commit the crime of using AI for unit testing however)
 - WHen it comes to evaluation techniques, I want to try learning from older articles/papers and will try to remember to reference them on the readme.
 - Test Driven Development:
   - Going to rely on tests to make sure everything is working as expected using real positions.
   - Also, it will help when I get to the optimization stages of development
 - I want to try and keep records of benchmark results for each version of the bot to keep track of improvements and test bots against each-other

# The plan
## Phase One: Get Something That Works
 - [ ] Get Basic Chess Engine Working With All The Rules Working
   - [x] Calculate Basic Legal Moves For All Pieces
   - [x] Pawn Shenanigans
     - [x] Pawn Promotion
     - [x] Diagonal Attacks
     - [x] En Passant
   - [ ] Castling
   - [x] Check And Checkmate
   - [x] Illegal Moves
   - [ ] Game Draws
     - [x] Stalemate
     - [ ] Threefold Repetition
     - [x] Fifty-Move Rule
     - [ ] Insufficient Material
 - [x] Abstract Input System (Player Input And Random Move Input As a Start) 
 - [ ] Basic MiniMax Evaluation Player Input

## Phase Two: Make It Not Suck
 - [ ] Investigate Optimization For Prev Step
 - [ ] Iterative Deepening
 - [ ] Openings
 - [ ] Optimize Core Engine
   - [ ] Use Bitboards
   - [ ] Revise Legal Move Generation (Assuming The First Iteration Is Bad And Precomputing Them With BitBoards Would Be Better)
 - [ ] Probably more
 - [ ] Make sure I can't beat it before continuing

## Phase Three: Make It Actually Good
- [ ] Determine some performance metric goal, beat it and move on to the next language