using UnityEngine;
using System.Collections;

public class CasinoCardsDeckShuffler {
	
	public enum TypeOfShuffler {Linear,FisherYates,InsideOut};	
	
	// Static Function to Shuffle
	public static int[] shuffleTheDeckUsing(TypeOfShuffler shufflerType, int[] deckOfCards)
	{
		int[] newDeckOfCards = null;
		switch(shufflerType)
		{
		case TypeOfShuffler.Linear:
			newDeckOfCards = shuffleTheDeckUsingLinearShuffle(deckOfCards);
			break;	
		case TypeOfShuffler.FisherYates:
			newDeckOfCards = shuffleTheDeckUsingFisherYatesShuffle(deckOfCards);
			break;		
		case TypeOfShuffler.InsideOut:
			newDeckOfCards = shuffleTheDeckUsingInsideOutShuffle(deckOfCards);
			break;		
		}
		return newDeckOfCards;
	}
	
	// DIFFERENT Shufflers	
	// Linear Shuffle : 
	// Assign a RandomNumber for each card and order the cards based on the assigned Number
	static int[] shuffleTheDeckUsingLinearShuffle(int[] deckOfCards)
	{
		int[] newDeckOfCards = new int[deckOfCards.Length];						// Creating a New Deck
		int[] randomNumbers = generateRandomNumbers(0, deckOfCards.Length-1);	// Generating the Nos from 0 to 52 arranged in a Random Manner
		for(int i=0; i<deckOfCards.Length; i++)									
			newDeckOfCards[i] = deckOfCards[randomNumbers[i]];					// Assigning the Nos of the new Deck by fetching the Nos from the 
																					// current deck using the index from the Random Nos generated
		return newDeckOfCards;				
	}
	
	// FisherYates Shuffle : 
	//	To shuffle an array a of n elements (indices 0..n-1):
	//  for i from n − 1 downto 1 do
	//       j = random integer with 0 ≤ j ≤ i
	//       exchange a[j] and a[i]
	static int[] shuffleTheDeckUsingFisherYatesShuffle(int[] deckOfCards)
	{
		int[] newDeckOfCards = new int[deckOfCards.Length];		// Creating a New Deck
		for(int i=deckOfCards.Length-1; i>=0; i--)				// for i from n − 1 downto 1 do
		{
			int j = Random.Range(0,i+1);						// j = random integer with 0 ≤ j ≤ i
			//   exchange a[j] and a[i]
			newDeckOfCards[i] = deckOfCards[j];
			deckOfCards[j] = deckOfCards[i];
			deckOfCards[i] = newDeckOfCards[i];
		}
		return newDeckOfCards;				
	}
	
	// InsideOut Algorithm Shuffle : 
	//	To initialize an array a of n elements to a randomly shuffled copy of source, both 0-based:
	//	  a[0] = source[0]
	//	  for i from 1 to n − 1 do
	//	      j = random integer with 0 ≤ j ≤ i
	//	      if j ≠ i
	//	          a[i] = a[j]
	//	      a[j] = source[i]
	static int[] shuffleTheDeckUsingInsideOutShuffle(int[] deckOfCards)
	{
		int[] newDeckOfCards = new int[deckOfCards.Length];		// Creating a New Deck
		for(int i=deckOfCards.Length-1; i>=0; i--)				// a[0] = source[0]
			newDeckOfCards[i] = deckOfCards[i];
		for(int i=1; i<deckOfCards.Length; i++)				// for i from 1 to n − 1 do
		{
			int j = Random.Range(0,i+1);						// j = random integer with 0 ≤ j ≤ i
			if(j != i)											// if j ≠ i
				newDeckOfCards[i] = newDeckOfCards[j];			// a[i] = a[j]
			newDeckOfCards[j] = deckOfCards[i];					// a[j] = source[i]
		}
		return newDeckOfCards;				
	}
	
	// Generating a series of Numbers from min to max arraged in a Random Manner
	public static int[] generateRandomNumbers(int min, int max, int noOfItems)
	{
		int arrCount = max - min + 1;
		if(noOfItems == 0) noOfItems = arrCount;
		int[] arrInt = new int[noOfItems];
		ArrayList arrTemp = new ArrayList();
		for(int i= 0 ; i< arrCount; i++)
			arrTemp.Add (min+i);
		
		int j = 0;
		while(noOfItems>0)
		{	
			int rand_ = Random.Range(0,arrCount--);
			arrInt[j] = (int)arrTemp[rand_];
			arrTemp.RemoveAt(rand_);
			j++;
			noOfItems--;
		}
		return arrInt;
	}
		
	public static int[] generateRandomNumbers(int min, int max)
	{
		return generateRandomNumbers(min, max, 0);
	}
}
