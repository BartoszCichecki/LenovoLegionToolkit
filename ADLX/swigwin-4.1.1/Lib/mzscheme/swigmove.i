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
      scheme_signal_error(FUNC_NAME ": cannot release ownership as memory is not owned for argument $argnum of type '$1_type'");
    } else {
      %argument_fail(res, "$1_type", $symname, $argnum);
    }
  }
  if (argp == NULL) scheme_signal_error(FUNC_NAME ": swig-type-error (null reference)");
  SwigValueWrapper< $1_ltype >::reset($1, ($&1_type)argp);
}
