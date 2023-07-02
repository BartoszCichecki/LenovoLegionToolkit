/* -----------------------------------------------------------------------------
 * swigmove.i
 *
 * Input typemaps library for implementing full move semantics when passing
 * parameters by value.
 * ----------------------------------------------------------------------------- */

%typemap(in, noblock=1) SWIGTYPE MOVE (void *argp = 0, int res = 0) {
  res = SWIG_ConvertPtr(&$input, &argp, $&1_descriptor, SWIG_POINTER_RELEASE);
  if (!SWIG_IsOK(res)) {
    if (res == SWIG_ERROR_RELEASE_NOT_OWNED) {
      zend_type_error("Cannot release ownership as memory is not owned for argument $argnum of $&1_descriptor of $symname");
      return;
    } else {
      zend_type_error("Expected $&1_descriptor for argument $argnum of $symname");
      return;
    }
  }
  if (!argp) {
    zend_type_error("Invalid null reference for argument $argnum of $&1_descriptor of $symname");
    return;
  }
  SwigValueWrapper< $1_ltype >::reset($1, ($&1_type)argp);
}
