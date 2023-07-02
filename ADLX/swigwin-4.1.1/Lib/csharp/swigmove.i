/* -----------------------------------------------------------------------------
 * swigmove.i
 *
 * Input typemaps library for implementing full move semantics when passing
 * parameters by value.
 * ----------------------------------------------------------------------------- */

%typemap(in, canthrow=1, fragment="<memory>") SWIGTYPE MOVE ($&1_type argp)
%{ argp = ($&1_ltype)$input;
   if (!argp) {
     SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "Attempt to dereference null $1_type", 0);
     return $null;
   }
   SwigValueWrapper< $1_ltype >::reset($1, argp); %}

%typemap(csin) SWIGTYPE MOVE "$&csclassname.swigRelease($csinput)"
