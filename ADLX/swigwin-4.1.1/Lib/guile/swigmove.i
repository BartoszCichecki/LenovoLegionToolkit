/* -----------------------------------------------------------------------------
 * swigmove.i
 *
 * Input typemaps library for implementing full move semantics when passing
 * parameters by value.
 * ----------------------------------------------------------------------------- */

%typemap(in, noblock=1) SWIGTYPE MOVE (void *argp = 0, int res = 0) {
  res = SWIG_ConvertPtr($input, &argp, $&1_descriptor, SWIG_POINTER_RELEASE);
  if (!SWIG_IsOK(res)) {
    if (res == SWIG_ERROR_RELEASE_NOT_OWNED) {
      %releasenotowned_fail(res, "$1_type", $symname, $argnum);
    } else {
      %argument_fail(res, "$1_type", $symname, $argnum);
    }
  }
  if (!argp) { %argument_nullref("$1_type", $symname, $argnum); }
  SwigValueWrapper< $1_ltype >::reset($1, ($&1_type)argp);
}
