/* -----------------------------------------------------------------------------
 * swigmove.i
 *
 * Input typemaps library for implementing full move semantics when passing
 * parameters by value.
 * ----------------------------------------------------------------------------- */

%typemap(in, checkfn="lua_isuserdata", noblock=1) SWIGTYPE MOVE (void *argp = 0, int res = 0) {
  res = SWIG_ConvertPtr(L, $input, &argp, $&1_descriptor, SWIG_POINTER_RELEASE);
  if (!SWIG_IsOK(res)) {
    if (res == SWIG_ERROR_RELEASE_NOT_OWNED) {
      lua_pushfstring(L, "Cannot release ownership as memory is not owned for argument $argnum of type '$1_type' in $symname"); SWIG_fail;
    } else {
      SWIG_fail_ptr("$symname", $argnum, $&1_descriptor);
    }
  }
  SwigValueWrapper< $1_ltype >::reset($1, ($&1_type)argp);
}
