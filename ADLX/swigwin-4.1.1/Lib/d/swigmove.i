/* -----------------------------------------------------------------------------
 * swigmove.i
 *
 * Input typemaps library for implementing full move semantics when passing
 * parameters by value.
 * ----------------------------------------------------------------------------- */

%typemap(in, canthrow=1) SWIGTYPE MOVE ($&1_type argp)
%{ argp = ($&1_ltype)$input;
   if (!argp) {
     SWIG_DSetPendingException(SWIG_DIllegalArgumentException, "Attempt to dereference null $1_type");
     return $null;
   }
   SwigValueWrapper< $1_ltype >::reset($1, argp); %}

%typemap(din) SWIGTYPE MOVE "$dclassname.swigRelease($dinput)"
