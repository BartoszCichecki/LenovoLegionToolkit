/* -----------------------------------------------------------------------------
 * std_unique_ptr.i
 *
 * SWIG library file for handling std::unique_ptr.
 * Memory ownership is passed from the std::unique_ptr C++ layer to the proxy
 * class when returning a std::unique_ptr from a function.
 * Memory ownership is passed from the proxy class to the std::unique_ptr in the
 * C++ layer when passed as a parameter to a wrapped function.
 * ----------------------------------------------------------------------------- */

%define %unique_ptr(TYPE)
%typemap (ctype) std::unique_ptr< TYPE > "void *"
%typemap (imtype) std::unique_ptr< TYPE > "void*"
%typemap (dtype) std::unique_ptr< TYPE > "$typemap(dtype, TYPE)"

%typemap(in) std::unique_ptr< TYPE >
%{ $1.reset((TYPE *)$input); %}

%typemap(din,
  nativepointer="cast(void*)$dinput"
) std::unique_ptr< TYPE > "$typemap(dtype, TYPE).swigRelease($dinput)"

%typemap (out) std::unique_ptr< TYPE > %{
  $result = (void *)$1.release();
%}

%typemap(dout, excode=SWIGEXCODE,
  nativepointer="{\n  auto ret = cast($dtype)$imcall;$excode\n  return ret;\n}"
) std::unique_ptr< TYPE > {
  void* cPtr = $imcall;
  $typemap(dtype, TYPE) ret = (cPtr is null) ? null : new $typemap(dtype, TYPE)(cPtr, true);$excode
  return ret;
}

%typemap(typecheck, precedence=SWIG_TYPECHECK_POINTER, equivalent="TYPE *") std::unique_ptr< TYPE > ""

%template() std::unique_ptr< TYPE >;
%enddef

namespace std {
  template <class T> class unique_ptr {};
}
