/* -----------------------------------------------------------------------------
 * swigmove.i
 *
 * Input typemaps library for implementing full move semantics when passing
 * parameters by value.
 * ----------------------------------------------------------------------------- */

%typemap(in, noblock=1) SWIGTYPE MOVE (void *argp = 0) {
  argp1 = ($&1_ltype) caml_ptr_val($input,$&1_descriptor);
  SwigValueWrapper< $1_ltype >::reset($1, ($&1_type)argp);
}
