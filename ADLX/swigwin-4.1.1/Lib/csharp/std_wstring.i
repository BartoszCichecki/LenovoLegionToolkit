/* -----------------------------------------------------------------------------
 * std_wstring.i
 *
 * Typemaps for std::wstring and const std::wstring&
 * std::wstring is mapped to a C# Unicode string (UTF16) and is passed around by value.
 * std::wstring support includes wchar_t as a 2 byte type (Windows) and a 4 byte type
 * (most Unix systems).
 *
 * To use non-const std::wstring references use the following %apply.  Note 
 * that they are passed by value.
 * %apply const std::wstring & {std::wstring &};
 * ----------------------------------------------------------------------------- */

%include <wchar.i>

%{
#include <string>
%}

%fragment("Swig_csharp_UTF16ToWString", "header") %{
/* For converting from .NET UTF16 (2 byte unicode) strings. wchar_t is 2 bytes on Windows, 4 bytes on Linux. */
static std::wstring Swig_csharp_UTF16ToWString(const unsigned short *str) {
  if (sizeof(wchar_t) == 2) {
    return std::wstring((wchar_t *)str);
  } else {
    const unsigned short *pBegin(str);
    const unsigned short *ptr(pBegin);

    while (*ptr != 0)
      ++ptr;

    std::wstring result;
    result.reserve(ptr - pBegin);
    while(pBegin != ptr)
      result.push_back(*pBegin++);

    return result;
  }
}
%}

namespace std {

%naturalvar wstring;

class wstring;

// wstring
%typemap(ctype, out="void *") wstring "unsigned short *"
%typemap(imtype,
         inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         directorinattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         directoroutattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]"
         ) wstring "string"
%typemap(cstype) wstring "string"
%typemap(csdirectorin) wstring "$iminput"
%typemap(csdirectorout) wstring "$cscall"

%typemap(in, canthrow=1, fragment="Swig_csharp_UTF16ToWString") wstring
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null wstring", 0);
    return $null;
   }
   $1 = Swig_csharp_UTF16ToWString($input); %}
%typemap(out) wstring %{ $result = SWIG_csharp_wstring_with_length_callback($1.c_str(), (int)$1.size()); %}

%typemap(directorout, canthrow=1) wstring
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null wstring", 0);
    return $null;
   }
   $result = Swig_csharp_UTF16ToWString($input); %}

%typemap(directorin) wstring %{ $input = SWIG_csharp_wstring_with_length_callback($1.c_str(), (int)$1.size()); %}

%typemap(csin) wstring "$csinput"
%typemap(csout, excode=SWIGEXCODE) wstring {
    string ret = $imcall;$excode
    return ret;
  }

%typemap(typecheck) wstring = wchar_t *;

%typemap(throws, canthrow=1) wstring
%{ SWIG_csharp_ApplicationException_callback($1.c_str(), (int)$1.size());
   return $null; %}

// const wstring &
%typemap(ctype, out="void *") const wstring & "unsigned short *"
%typemap(imtype,
         inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         directorinattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         directoroutattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]"
         ) const wstring & "string"
%typemap(cstype) const wstring & "string"

%typemap(csdirectorin) const wstring & "$iminput"
%typemap(csdirectorout) const wstring & "$cscall"

%typemap(in, canthrow=1, fragment="Swig_csharp_UTF16ToWString") const wstring &
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null wstring", 0);
    return $null;
   }
   std::wstring $1_str(Swig_csharp_UTF16ToWString($input));
   $1 = &$1_str; %}
%typemap(out) const wstring & %{ $result = SWIG_csharp_wstring_with_length_callback($1->c_str(), (int)$1->size()); %}

%typemap(csin) const wstring & "$csinput"
%typemap(csout, excode=SWIGEXCODE) const wstring & {
    string ret = $imcall;$excode
    return ret;
  }

%typemap(directorout, canthrow=1, warning=SWIGWARN_TYPEMAP_THREAD_UNSAFE_MSG) const wstring &
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null wstring", 0);
    return $null;
   }
   /* possible thread/reentrant code problem */
   static std::wstring $1_str;
   $1_str = Swig_csharp_UTF16ToWString($input);
   $result = &$1_str; %}

%typemap(directorin) const wstring & %{ $input = SWIG_csharp_wstring_with_length_callback($1.c_str(), (int)$1.size()); %}

%typemap(csvarin, excode=SWIGEXCODE2) const wstring & %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) const wstring & %{
    get {
      string ret = $imcall;$excode
      return ret;
    } %}

%typemap(typecheck) const wstring & = wchar_t *;

%typemap(throws, canthrow=1) const wstring &
%{ SWIG_csharp_ApplicationException_callback($1.c_str(), (int)$1.size());
   return $null; %}

}

