/* -----------------------------------------------------------------------------
 * std_auto_ptr.i
 *
 * SWIG library file for handling std::auto_ptr.
 * Memory ownership is passed from the std::auto_ptr C++ layer to the proxy
 * class when returning a std::auto_ptr from a function.
 * Memory ownership is passed from the proxy class to the std::auto_ptr in the
 * C++ layer when passed as a parameter to a wrapped function.
 * ----------------------------------------------------------------------------- */

%define %auto_ptr(TYPE)
%typemap (ctype) std::auto_ptr< TYPE > "void *"
%typemap (imtype) std::auto_ptr< TYPE > "void*"
%typemap (dtype) std::auto_ptr< TYPE > "$typemap(dtype, TYPE)"

%typemap(in) std::auto_ptr< TYPE >
%{ $1.reset((TYPE *)$input); %}

%typemap(din,
  nativepointer="cast(void*)$dinput"
) std::auto_ptr< TYPE > "$typemap(dtype, TYPE).swigRelease($dinput)"

%typemap (out) std::auto_ptr< TYPE > %{
  $result = (void *)$1.release();
%}

%typemap(dout, excode=SWIGEXCODE,
  nativepointer="{\n  auto ret = cast($dtype)$imcall;$excode\n  return ret;\n}"
) std::auto_ptr< TYPE > {
  void* cPtr = $imcall;
  $typemap(dtype, TYPE) ret = (cPtr is null) ? null : new $typemap(dtype, TYPE)(cPtr, true);$excode
  return ret;
}

%typemap(typecheck, precedence=SWIG_TYPECHECK_POINTER, equivalent="TYPE *") std::auto_ptr< TYPE > ""

%template() std::auto_ptr< TYPE >;
%enddef

namespace std {
  template <class T> class auto_ptr {};
}
