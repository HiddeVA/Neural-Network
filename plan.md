# Create your own AI - Neural Networks

## Requirements

- A dataset
- Code
- Presentation

## Sources

- MNIST data set https://huggingface.co/datasets/ylecun/mnist
- http://neuralnetworksanddeeplearning.com/

## Presentation outline

- Terminology: AI, Neural Networks, Deep Learning, LLMs
- Problems thats can be solved with neural networks
- Setting up the code

## The code

Source: https://github.com/mnielsen/neural-networks-and-deep-learning/blob/master/src/network.py
- Calculate gradient descent for each weight/bias
1. Calculate the result vectors and activations for each layer
2. Start at the last layer
3. Delta for biases is sigmoid_prime * cost func derivative
4. Delta for weights?