# Binary Pigeon Inspired Optimization (BPIO) for the Knapsack problem

If you're using BPIO for the knapsack problem, the following startup flags have been added:

To Download, check [this section](../README.md#command-line-interface) for instructions.

### How to use

The Command Line for this application makes use of flags, which are described in [this section](../README.md#how-to-use).

New Flags available include:

#### Algorithm Flag

With this flag, you can switch between Flower Pollination and BPIO Algorithms.

- Flower Pollination Algorithm: `-algo 1` or `-algorithm 1`
- Binary Pigeon Inspired Optimization: `-algo 2` or `-algorithm 2`

If not specified, the value of `Algorithm` defaults to `Flower Pollination Algorithm`

#### TableSize Flag

The BPIO Algorithm may use Late Acceptance principle. You can set the `TableSize` variable. 

Use `-t 150` or `-tableSize 150` to change the value of `TableSize` to `150`

If not specified, the value of `TableSize` defaults to `1`

#### MapFactor Flag

With this flag, you can modify the `MapFactor` variable that controls the BPIO algorithm. 

Use `-m 0.15` or `-map 0.15` to change the value of `MapFactor` to `0.15`

If not set, the value of `MapFactor` defaults to `0.5`

#### NoOfIterations Flag

With this flag, you can modify the `NoOfIterations` variable that controls the BPIO algorithm. 

Use `-i 1500` or `-iterations 1500` to change the value of `NoOfIterations` to `1500`

If not set, the value of `NoOfIterations` defaults to `50000`
