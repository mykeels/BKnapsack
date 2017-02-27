# Multi-Knapsack Problem

In the multiple knapsack problem, we have multiple knapsacks or bags, each can carry a maximum weight. E.g. 10kg.

We also have a number of items we can choose from to carry in the knapsacks.

For each item we choose, we have to place it in all knapsacks.

Once a knapsack is full, we can't select items again.

Each item also has a value to the knapsack-owner, and a different weight in each knapsack. 

```
E.g. a gold-wristwatch can be worth $5000 and have a weight of 1kg in knapsack 1, 3kg in knapsack 2 and 500g in knapsack 3.
```

_Don't look at me, I didn't invent the rules :)_



E.g.

```
noOfKnapsacks: 3
noOfItems: 3
knapsackWeights: [100, 120, 80]
itemValue: [
    {
        value: 5000,
        weights: [30, 32, 43]
    },
    {
        value: 5000,
        weights: [30, 32, 43]
    },
    {
        value: 5000,
        weights: [30, 32, 43]
    }
]
```

Part of the data above is JSON ... for more information, [see tutorial](http://www.elated.com/articles/json-basics/)

For more information on the dataset-format, see the [dataset](data) we used.

#### Solution Representation

A simple way to represent a candidate solution to the knapsack problem is an array or string of bits (0 or 1s).

Each bit points to a unique item in the problem and represents whether or that item was selected/chosen.

For the problem described above, a simple solution could be `[1, 1, 0]` or `110` which means items 1 and 2 were chosen, or even `[1, 0, 1]` or `101` which means items 1 and 3 were chosen.

#### Quality of a solution

Note that a solution's quality is represented by the total value of items chosen. Also, our goal is to improve/increase the quality of the solution as much as possible by selecting the items with the highest possible value.

You'd think we could just select all items yea? No, sadly. We have to make sure that the total weights of the items do not exceed the maximum weight of the corresponding knapsack. This is known as the hard-objective, and this should never be violated.
