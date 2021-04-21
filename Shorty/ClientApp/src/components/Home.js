import React, { Component } from 'react';
import {ApiError, getShortenedLink, getStats} from "../api";
import "./Home.scss";

const PLACEHOLDERS = [
    'yandex.ru',
    'google.com',
    'youtube.com',
    'facebook.com',
    'youtube.com/watch?v=dQw4w9WgXcQ'
];

const RANDOM_PLACEHOLDER = PLACEHOLDERS[Math.floor(Math.random() * PLACEHOLDERS.length)]

export class Home extends Component {
    static displayName = Home.name;
    state = {
        link: '',
        loading: false,
        errors: null,
        showSidebar: false,
        links: [],
        stats: null
    }
    /**
     * @type React.RefObject<HTMLInputElement>
     */
    inputRef = React.createRef();
    
    componentDidMount() {
        try {
            const links = JSON.parse(localStorage['links'])
            this.setState({links})
        } catch (e) {}
        
        getStats().then(stats => this.setState({stats}))
    }

    saveLinks() {
        localStorage['links'] = JSON.stringify(this.state.links)
    }

    clearHistory() {
        this.setState({links: []})
        this.forceUpdate(() => this.saveLinks())
    }

    async generate () {
    if (this.state.loading)
        return;
    try {
        const link = this.state.link.trim();
        this.setState({link})
        const info = await getShortenedLink(link);
        
        const links = [...this.state.links]
        links.unshift({
            addr: info.addr,
            id: info.id
        })
        
        this.setState({
            link: location.origin + '/' + info.id,
            loading: false,
            error: null,
            links
        })
        
        this.forceUpdate(() => this.saveLinks())
    } catch (e) {
        let error = e.toString();
        if (e instanceof ApiError) {
            error = e.errors;
        }
        this.setState({
            loading: false,
            error
        })
    }
    }
    
    render () {
    return (
      <div className="is-flex is-flex-direction-column is-align-items-center home">
          <h1 className="app-title">Shorty</h1>
          <div className="url-main">
              {this.state.link.trim() !== '' ? <button
                  onClick={() => this.inputRef.current.select() || document.execCommand("copy")}
                  className="url-main__copy">
                  <i className="fas fa-clone"/>
              </button> : undefined}
              <input
                  ref={this.inputRef}
                  disabled={this.state.loading}
                  onFocus={e => e.target.select()}
                  value={this.state.link}
                  placeholder={RANDOM_PLACEHOLDER}
                  className="url-main__input"
                  onChange={e => this.setState({link: e.target.value})}
                  type="text"/>
          </div>
          {this.state.error ? this.renderErrors(this.state.error) : undefined}
          <button
              disabled={this.state.link.trim() === ''}
              className={"button is-primary" + (this.state.loading ? ' is-loading ' : '')}
              onClick={() => this.generate()}>Shortify</button>
          
          <div className="home__bottom">
              {this.state.stats ? <div>
                  {this.state.stats.totalCount} links shortened so far</div> : undefined}
          </div>
          
          
          <button className="home__open-sidebar"
            onClick={() => this.setState(() => this.setState({showSidebar: !this.state.showSidebar}))}>
              <i className={'fas ' + (this.state.showSidebar ? 'fa-times' : 'fa-bars')}/>
          </button>
          {this.state.showSidebar && this.renderSideBar()}
      </div>
    );
    }
    
    renderSideBar() {
      return <div className="home__sidebar-wrapper">
          <div className="sidebar">
              <button 
                  onClick={() => this.clearHistory()}
                  className="button is-text mb-2">Clear</button>
              {this.state.links.map(info => <div className="sidebar-item" key={info.id}>
                  <a className="sidebar-item__shorty" href={location.origin + '/' + info.id}>
                      <span className="origin">{location.origin}/</span>{info.id}
                  </a>
                  <div className="sidebar-item__link">{info.addr}</div>
              </div>)}
          </div>
      </div>
    }
    
    renderErrors(errors) {
      const items = [];
      
      let index = 0;
      for (let field in errors) {
          for (let err of errors[field]) {
              items.push(
                  <div className="error" key={index}>
                      <span className="error__field">{field}</span>
                      <span className="error__body">{err}</span>
                  </div>
              );
              index++;
          }
      }
      
      return <div className="errors">{items}</div>
    }
}
