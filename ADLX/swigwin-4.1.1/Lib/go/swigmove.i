/* -----------------------------------------------------------------------------
 * swigmove.i
 *
 * Input typemaps library for implementing full move semantics when passing
 * parameters by value.
 * ----------------------------------------------------------------------------- */

%typemap(in) SWIGTYPE MOVE ($&1_type argp)
%{
  argp = ($&1_ltype)$input;
  if (argp == NULL) {
    _swig_gopanic("Attempt to dereference null $1_type");
  }
  SwigValueWrapper< $1_ltype >::reset($1, argp);
%}
