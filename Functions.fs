namespace Albelli

    module Functions =
        let uncurry f (a, b) = f a b
        let curry f a b = f(a, b)