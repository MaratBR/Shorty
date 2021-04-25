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
        links: [],
        stats: null,
        aliased: false,
        alias: ''
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
        const info = await getShortenedLink(link, this.state.aliased ? this.state.alias : undefined);
        
        const links = [...this.state.links]
        links.unshift({
            addr: info.addr,
            id: info.id
        })
        
        this.setState({
            link: window.location.origin + '/' + info.id,
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
        const hasLink = this.state.link.trim() !== '' && (!this.state.aliased || this.state.alias.trim() !== '')
        return (
          <div className="is-flex is-flex-direction-column is-align-items-start home">
              <div className="home__top">
                  <h1 className="app-title">Shorty</h1>
              </div>
              <section className="url-main">
                  <div className="url-main__type">
                      <button 
                          onClick={() => this.setState({aliased: false})}
                          className={'button tag' + (!this.state.aliased ? ' is-primary' : '')}>Normal</button>
                      <button
                          onClick={() => this.setState({aliased: true})}
                          className={'button tag' + (this.state.aliased ? ' is-primary' : '')}>Aliased</button>
                  </div>
                  <div className="url-main__shortener">
                      <i onClick={() => this.inputRef.current.focus()} className="fas fa-arrow-right url-main__prefix" />
    
                      <input
                          ref={this.inputRef}
                          disabled={this.state.loading}
                          onFocus={e => e.target.select()}
                          value={this.state.link}
                          placeholder={RANDOM_PLACEHOLDER}
                          className="url-main__input"
                          onChange={e => this.setState({link: e.target.value})}
                          type="text"/>
    
                      {this.state.aliased ? <input
                          disabled={this.state.loading}
                          value={this.state.alias}
                          placeholder="Alias"
                          className="url-main__alias"
                          onChange={e => this.setState({alias: e.target.value})}
                          type="text"/> : undefined}
    
                      {hasLink ? <button
                          onClick={() => this.inputRef.current.select() || document.execCommand("copy")}
                          className="url-main__copy">
                          <i className="fas fa-clone"/>
                      </button> : undefined}
    
                      <button
                          disabled={!hasLink}
                          className={"url-main__btn button is-text is-rounded" + (this.state.loading ? ' is-loading ' : '')}
                          onClick={() => this.generate()}>Shortify</button>
                  </div>
              </section>
              {this.state.error ? this.renderErrors(this.state.error) : undefined}
              
              
              <div className="home__bottom">
                  {this.state.stats ? `${this.state.stats.totalCount} links shortened so far` : ''}
              </div>
              
              {this.renderLinks()}
          </div>
        );
    }
    
    renderLinks() {
      return <div className="links">
          <button 
              onClick={() => this.clearHistory()}
              className="button is-text mb-2">Clear</button>
          {this.state.links.map(info => <div className="sidebar-item" key={info.id}>
              <a className="sidebar-item__shorty" href={window.location.origin + '/' + info.id}>
                  <span className="origin">{window.location.origin}/</span>{info.id}
              </a>
              <div className="sidebar-item__link">{info.addr}</div>
          </div>)}
      </div>
    }
    
    renderErrors(errors) {
        const items = [];
        
        if (typeof errors === 'string') {
            items.push(<div className="error" >
                <span className="error__body">{errors}</span>
            </div>)
        } else if (errors instanceof Array) {
            for (let i = 0; i < errors.length; i++) {
                items.push(
                    <div className="error" key={i}>
                        <span className="error__body">{errors[i]}</span>
                    </div>
                );
            }
        } else {
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
        }
        
        return <div className="errors">{items}</div>
    }
}
