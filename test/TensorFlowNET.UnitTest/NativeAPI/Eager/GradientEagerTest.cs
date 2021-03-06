﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tensorflow;
using static Tensorflow.Binding;

namespace TensorFlowNET.UnitTest.Gradient
{
    [TestClass]
    public class GradientEagerTest : PythonTest
    {
        [TestMethod]
        public void ConstantSquare()
        {
            // Calcute the gradient of w * w 
            // by Automatic Differentiation in Eager mode
            // in tensorflow.net 2.x that is in development intensively
            var w = tf.constant(1.5f);
            using var tape = tf.GradientTape();
            tape.watch(w);
            var loss = w * w;
            var grad = tape.gradient(loss, w);
            Assert.AreEqual((float)grad, 3.0f);
        }

        [TestMethod]
        public void ConstantMultiply()
        {
            var x = tf.ones((2, 2));
            using var tape = tf.GradientTape();
            tape.watch(x);
            var y = tf.reduce_sum(x);
            var z = tf.multiply(y, y);
            var dz_dx = tape.gradient(z, x);

            var expected = new float[] { 8.0f, 8.0f, 8.0f, 8.0f };
            Assert.IsTrue(Enumerable.SequenceEqual(dz_dx.ToArray<float>(), expected));
        }

        [TestMethod]
        public void PersistentTape()
        {
            var x = tf.ones((2, 2));
            using var tape = tf.GradientTape(persistent: true);
            tape.watch(x);
            var y = tf.reduce_sum(x);
            var z = tf.multiply(y, y);
            tape.Dispose();

            var dz_dx = tape.gradient(z, x);

            var expected = new float[] { 8.0f, 8.0f, 8.0f, 8.0f };
            Assert.IsTrue(Enumerable.SequenceEqual(dz_dx.ToArray<float>(), expected));

            var dz_dy = tape.gradient(z, y);
            Assert.AreEqual((float)dz_dy, 8.0f);
        }
    }
}
