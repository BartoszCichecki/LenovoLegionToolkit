%define %pythonabc(Type, Abc)
  %feature("python:abc", Abc) Type;
%enddef
%pythoncode %{if _swig_python_version_info[0:2] >= (3, 3):
    import collections.abc
else:
    import collections
%}
%pythonabc(std::vector, "collections.abc.MutableSequence if _swig_python_version_info >= (3, 3) else collections.MutableSequence");
%pythonabc(std::list, "collections.abc.MutableSequence if _swig_python_version_info >= (3, 3) else collections.MutableSequence");
%pythonabc(std::map, "collections.abc.MutableMapping if _swig_python_version_info >= (3, 3) else collections.MutableMapping");
%pythonabc(std::multimap, "collections.abc.MutableMapping if _swig_python_version_info >= (3, 3) else collections.MutableMapping");
%pythonabc(std::set, "collections.abc.MutableSet if _swig_python_version_info >= (3, 3) else collections.MutableSet");
%pythonabc(std::multiset, "collections.abc.MutableSet if _swig_python_version_info >= (3, 3) else collections.MutableSet");
